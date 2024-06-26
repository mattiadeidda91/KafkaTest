using Common.Interfaces;

namespace Common.Models.Weather
{
    public class WeatherForecastStartedEvent : BaseEvent<WeatherForecast>, IEvent
    {
    }
}
