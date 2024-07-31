using Common.Models.Order;
using Common.Models.Weather;
using Kafka.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers.v1
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

        //[Obsolete("replace in v2 version")]
        [HttpPost("PostWeather")]
        public async Task<ActionResult<WeatherForecast>> PostWeather(WeatherForecast weatherForecastRequest, CancellationToken cancellationToken)
        {
            (var message, var headers) = msgBus.GenerateMsgBusEvent<WeatherForecastStartedEvent, WeatherForecast>(weatherForecastRequest);

            await msgBus.SendMessage(message, null, headers);

            return Ok(weatherForecastRequest);
        }

        [HttpPost("PostOrder")]
        public async Task<ActionResult<Order>> PostOrder(Order order, CancellationToken cancellationToken)
        {
            (var message, var headers) = msgBus.GenerateMsgBusEvent<OrderStartedEvent, Order>(order);

            await msgBus.SendMessage(message, null, headers);

            return Ok(order);
        }
    }
}
