using Common.Models.Order;
using Common.Models.Weather;
using Consumer.Handlers;
using Common.Interfaces;

namespace Consumer.Configuration
{
    public static class EventHandlerBuilderExtensions
    {
        public static IServiceCollection ConfigureEventHandlerMsgBus(this IServiceCollection services)
        {
            services.AddScoped<IEventHandler<WeatherForecastStartedEvent>, WeatherForecastStartedHandler>();
            services.AddScoped<IEventHandler<OrderStartedEvent>, OrderStartedHandler>();

            return services;
        }
    }
}
