using Common.Models.Order;
using Common.Models.Weather;
using Kafka.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IEventBusService _msgBus;

        public WeatherForecastController(IEventBusService msgBus)
        {
            this._msgBus = msgBus;
        }

        [HttpPost("PostWeather")]
        public async Task<ActionResult<WeatherForecast>> PostWeather(WeatherForecast weatherForecastController)
        {
            (var message, var headers) = _msgBus.GenerateMsgBusEvent<WeatherForecastStartedEvent, WeatherForecast>(weatherForecastController);

            await _msgBus.SendMessage(message, null, headers);

            return Ok(weatherForecastController);
        }

        [HttpPost("PostOrder")]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            (var message, var headers) = _msgBus.GenerateMsgBusEvent<OrderStartedEvent, Order>(order);

            await _msgBus.SendMessage(message, null, headers);

            return Ok(order);
        }
    }
}