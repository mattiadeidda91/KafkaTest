using Common.Interfaces;
using Common.Logger;
using Common.Utils;
using Kafka.Interfaces;
using System.Text;
using System.Text.Json;

namespace Consumer.HostedService
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<KafkaConsumerService> logger;

        public KafkaConsumerService(IServiceProvider serviceProvider, ILogger<KafkaConsumerService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {          
            using (var scope = serviceProvider.CreateScope())
            {
                var eventBusService = scope.ServiceProvider.GetService<IEventBusService>();

                await eventBusService!.Subscribe(async (msg) =>
                {
                    logger.LogInfoSubscribeKafkaTopic();

                    var headers = msg.Message.Headers;
                    //var message = msg.Message.Value;
                    //var topic = msg.Topic;
                    //var offset = msg.Offset;
                    //var partition = msg.Partition;

                    if (headers != null && headers.Count > 0)
                    {
                        //Get event class namespace from kafka headers message
                        var eventTypeString = Encoding.UTF8.GetString(headers.First(h => h.Key.Equals("namespace")).GetValueBytes());
                        var classType = TypeResolver.GetEventType(eventTypeString);

                        if (classType != null)
                        {
                            //Deserialize object to specific class type
                            var eventInstance = JsonSerializer.Deserialize(msg.Message.Value, classType);

                            if (eventInstance != null)
                            {
                                //Get handler type for specific event to manage
                                var handlerType = typeof(IEventHandler<>).MakeGenericType(classType);
                                using (var scope = serviceProvider.CreateScope())
                                {
                                    var handler = scope.ServiceProvider.GetService(handlerType);
                                    var handleMethod = handlerType.GetMethod("HandleAsync");
                                    if (handler != null && handleMethod != null)
                                    {
                                        logger.LogInfoHandlerInvoke(handler.GetType().Name);

                                        //Invoke handler
                                        await (Task)handleMethod!.Invoke(handler, new[] { eventInstance });
                                    }
                                }
                            }
                            else
                            {
                                logger.LogWarningDeserializeKafkaEvent(classType.Name);
                            }
                        }
                        else
                        {
                            logger.LogWarningEventTypeNotFound(eventTypeString);
                        }
                    }
                    else
                    {
                        logger.LogWarningHeadersNotFound();
                    }

                    await Task.CompletedTask;
                });
            }
        }
    }
}
