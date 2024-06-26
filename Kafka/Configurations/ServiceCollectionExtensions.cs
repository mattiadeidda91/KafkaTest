using Kafka.Interfaces;
using Kafka.Options;
using Kafka.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBusService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaOptions>(configuration.GetSection(nameof(KafkaOptions)));
            services.AddScoped<IEventBusService, EventBusService>();

            return services;
        }
    }
}
