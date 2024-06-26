using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}