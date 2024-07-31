using Common.Models.Order;
using Common.Models.Weather;
using Kafka.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IEventBusService msgBus;

        public WeatherForecastController(IEventBusService msgBus)
        {
            this.msgBus = msgBus;
        }

        [HttpPost("PostWeather")]
        public async Task<ActionResult<WeatherForecast>> PostWeather(WeatherForecast weatherForecastRequest, string text, CancellationToken cancellationToken)
        {
            (var message, var headers) = msgBus.GenerateMsgBusEvent<WeatherForecastStartedEvent, WeatherForecast>(weatherForecastRequest);

            await msgBus.SendMessage(message, null, headers);

            return Ok(weatherForecastRequest);
        }
    }
}