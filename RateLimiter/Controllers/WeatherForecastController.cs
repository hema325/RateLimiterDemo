using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimiter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[EnableRateLimiting("FixedWindowLimiter")]
    //[EnableRateLimiting("SlidingWindowLimiter")]
    //[EnableRateLimiting("TokenBucketLimiter")]
    //[EnableRateLimiting("ConcurrencyLimiter")]
    [EnableRateLimiting("IPAddressLimiter")]

    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        private static int count = 0;
        private static DateTime dateTime = DateTime.Now;
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var taskNum = ++count;
            _logger.LogInformation($"Task {taskNum} at " + (DateTime.Now - dateTime).TotalSeconds.ToString("n2"));

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
