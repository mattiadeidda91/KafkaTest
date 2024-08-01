using Common.Interfaces;
using Common.Models.Weather;
using System.Text.Json;

namespace Consumer.Handlers
{
    public class WeatherForecastStartedHandler : IEventHandler<WeatherForecastStartedEvent>
    {
        public Task HandleAsync(WeatherForecastStartedEvent weatherEvent)
        {
            Console.WriteLine($"Handling WeatherForecastStartedEvent: {weatherEvent.ActivityId}, {JsonSerializer.Serialize(weatherEvent.Activity)}");
            
            return Task.CompletedTask;
        }
    }
}
