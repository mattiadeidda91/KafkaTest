using Common.Interfaces;
using Common.Utils;
using Kafka.Interfaces;
using System.Text;
using System.Text.Json;

namespace Consumer.HostedService
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public KafkaConsumerService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {          
            using (var scope = serviceProvider.CreateScope())
            {
                var eventBusService = scope.ServiceProvider.GetService<IEventBusService>();

                await eventBusService!.Subscribe(async (msg) =>
                {
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
                                        //Invoke handler
                                        await (Task)handleMethod!.Invoke(handler, new[] { eventInstance });
                                    }
                                }
                            }
                        }
                    }

                    await Task.CompletedTask;
                });
            }
        }
    }
}
