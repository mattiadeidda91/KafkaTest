using Common.Models;
using Confluent.Kafka;
using Kafka.Interfaces;
using Kafka.Models;
using Kafka.Options;
using Kafka.Utils;
using Microsoft.Extensions.Options;
using System.Text;

namespace Kafka.Services
{
    public class EventBusService : IEventBusService
    {
        private readonly ClientConfig producerConfig;
        private readonly KafkaOptions kafkaOptions;

        public EventBusService(IOptions<KafkaOptions> options) 
        {
            this.kafkaOptions = options.Value;
            this.producerConfig = new ProducerConfig()
            {
                BootstrapServers = options.Value.Host,
                SecurityProtocol = SecurityProtocol.Plaintext
            };
        }

        private ConsumerConfig GetConsumerConfig(string? groupId)
        {
            return new ConsumerConfig()
            {
                BootstrapServers = this.kafkaOptions.Host,
                SecurityProtocol = SecurityProtocol.Plaintext,
                GroupId = string.IsNullOrWhiteSpace(groupId) ? Guid.Empty.ToString() : groupId,
                AutoOffsetReset=AutoOffsetReset.Earliest

            };
        }

        public async Task<DeliveryResult<string,T>> SendMessage<T>(T message, string? key = null, Headers? headers = null)
        {
            using var producer = new ProducerBuilder<string, T>(this.producerConfig)
                .SetValueSerializer(new JsonSerializer<T>())
                .Build();

            var kMessage = new KafkaMessage<T>(message);
            kMessage.Key = key;
            kMessage.Headers = headers;

            var ret = await producer.ProduceAsync(kafkaOptions.Topic, kMessage);
            producer.Flush();
            return ret;
        }

        public async Task Subscribe<T>(Action<ConsumeResult<string,T>> action, string? groupId= null)
        {
            using var consumer = new ConsumerBuilder<string, T>(this.GetConsumerConfig(groupId))
                .SetValueDeserializer(new JsonSerializer<T>())
                .Build();

            consumer.Subscribe(kafkaOptions.Topic);

            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(default(CancellationToken));
                        action(consumeResult);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });

        }

        public async Task Subscribe(Action<ConsumeResult<string, string>> action, string? groupId = null)
        {
            using var consumer = new ConsumerBuilder<string, string>(this.GetConsumerConfig(groupId)).Build();

            consumer.Subscribe(kafkaOptions.Topic);

            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(default(CancellationToken));
                        action(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        // Gestione delle eccezioni
                    }
                }
            });
        }

        public (TEvent, Headers) GenerateMsgBusEvent<TEvent, TActivity>(TActivity activity) 
            where TEvent : BaseEvent<TActivity>, new() where TActivity : class
        {
            var msgBusEvent = new TEvent
            {
                ActivityId = Guid.NewGuid().ToString(),
                Activity = activity
            };

            var msgBusHeaders = new Headers
            {
                { "namespace", Encoding.UTF8.GetBytes(typeof(TEvent).FullName!) }
            };

            return (msgBusEvent, msgBusHeaders);
        }
    }
}