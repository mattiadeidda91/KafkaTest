using Common.Interfaces;
using Common.Models.Weather;
using System.Text.Json;

namespace Consumer.Handlers
{
    public class WeatherForecastStartedHandler : IEventHandler<WeatherForecastStartedEvent>
    {
        public Task HandleAsync(WeatherForecastStartedEvent @event)
        {
            Console.WriteLine($"Handling WeatherForecastStartedEvent: {@event.ActivityId}, {JsonSerializer.Serialize(@event.Activity)}");
            
            return Task.CompletedTask;
        }
    }
}
