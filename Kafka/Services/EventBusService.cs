using Common.Logger;
using Common.Models;
using Confluent.Kafka;
using Kafka.Constants;
using Kafka.Interfaces;
using Kafka.Models;
using Kafka.Options;
using Kafka.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Kafka.Services
{
    public class EventBusService : IEventBusService
    {
        private readonly ClientConfig producerConfig;
        private readonly KafkaOptions kafkaOptions;
        private readonly ILogger<EventBusService> logger;

        public EventBusService(IOptions<KafkaOptions> options, ILogger<EventBusService> logger) 
        {
            this.logger = logger;
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

        public async Task<DeliveryResult<string,T>> SendMessage<T>(T message, CancellationToken cancellationToken, string? key = null, Headers? headers = null)
        {
            using var producer = new ProducerBuilder<string, T>(this.producerConfig)
                .SetValueSerializer(new JsonSerializer<T>())
                .Build();

            var kMessage = new KafkaMessage<T>(message);
            kMessage.Key = key;
            kMessage.Headers = headers;

            var ret = await producer.ProduceAsync(kafkaOptions.Topic, kMessage, cancellationToken);

            logger.LogInfoKafkaMessageSent(JsonSerializer.Serialize(kMessage));

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
                        logger.LogErrorKafkaMessageSent(ex.Message);
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
                        logger.LogErrorKafkaMessageSent(ex.Message);
                    }
                }
            });
        }

        public (TEvent, Headers) GenerateMsgBusEvent<TEvent, TActivity>(TActivity activity) 
            where TEvent : BaseEvent<TActivity>, new() where TActivity : class
        {
            var msgBusEvent = new TEvent
            {
                ActivityId = Guid.NewGuid(),
                Activity = activity
            };

            var msgBusHeaders = new Headers
            {
                { "namespace", Encoding.UTF8.GetBytes(typeof(TEvent).FullName!) }
            };

            return (msgBusEvent, msgBusHeaders);
        }

        public ActionResult<MessageBusResponse<TDto>> ManageBusResponse<TEvent, TDto>(ControllerBase controller, DeliveryResult<string, TEvent> deliveryResult, TDto? activityRequest)
                where TDto : BaseDto
                where TEvent : BaseEvent<TDto>
        {
            ArgumentNullException.ThrowIfNull(controller);
            ArgumentNullException.ThrowIfNull(activityRequest);

            return deliveryResult.Status switch
            {
                PersistenceStatus.Persisted => controller.Ok(new MessageBusResponse<TDto>
                {
                    ActivityId = activityRequest.Guid,
                    Title = KafkaCostants.ActivityInCharge,
                    Status = (int)HttpStatusCode.OK,
                    Detail = KafkaCostants.MessageSuccessfullyPersisted,
                    OriginalPayload = activityRequest
                }),

                PersistenceStatus.PossiblyPersisted => controller.StatusCode((int)HttpStatusCode.Accepted, new MessageBusResponse<TDto>
                {
                    ActivityId = activityRequest.Guid,
                    Title = KafkaCostants.ActivityInProgress,
                    Status = (int)HttpStatusCode.Accepted,
                    Detail = KafkaCostants.MessagePossiblyPersisted,
                    OriginalPayload = activityRequest
                }),

                PersistenceStatus.NotPersisted => controller.BadRequest(new MessageBusResponse<TDto>
                {
                    ActivityId = activityRequest.Guid,
                    Title = KafkaCostants.ActivityFailed,
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = KafkaCostants.MessageNotPersisted,
                    OriginalPayload = activityRequest
                }),

                _ => controller.BadRequest(new MessageBusResponse<TDto>
                {
                    ActivityId = activityRequest.Guid,
                    Title = KafkaCostants.ActivityFailed,
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = KafkaCostants.MessageUnknownStatus,
                    OriginalPayload = activityRequest
                }),
            };
        }
    }
}