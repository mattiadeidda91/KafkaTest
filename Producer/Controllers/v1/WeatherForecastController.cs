using Common.Models.Order;
using Common.Models.Weather;
using Kafka.Interfaces;
using Kafka.Models;
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
        public async Task<ActionResult<MessageBusResponse<WeatherForecast>>> PostWeather(WeatherForecast weatherForecastRequest, CancellationToken cancellationToken)
        {
            (var message, var headers) = msgBus.GenerateMsgBusEvent<WeatherForecastStartedEvent, WeatherForecast>(weatherForecastRequest);

            var responseMsgBus = await msgBus.SendMessage(message, cancellationToken, null, headers);

            return msgBus.ManageBusResponse(this, responseMsgBus, message.Activity);
        }

        [HttpPost("PostOrder")]
        public async Task<ActionResult<MessageBusResponse<Order>>> PostOrder(Order order, CancellationToken cancellationToken)
        {
            (var message, var headers) = msgBus.GenerateMsgBusEvent<OrderStartedEvent, Order>(order);

            var responseMsgBus = await msgBus.SendMessage(message, cancellationToken, null, headers);

            return msgBus.ManageBusResponse(this, responseMsgBus, message.Activity);
        }
    }
}
