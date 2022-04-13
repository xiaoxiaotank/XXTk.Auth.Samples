using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using XXTk.Auth.Samples.PolicyBased.HttpApi.Authorizations;

namespace XXTk.Auth.Samples.PolicyBased.HttpApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
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

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetForAtLeast18Age")]
        [Authorize(Policy = "AtLeast18Age")]
        public string GetForAtLeast18Age()
        {
            return "At least 18 age";
        }

        [HttpGet("GetForAtLeast20Age")]
        [MinimumAgeAuthorize(20)]
        public string GetForAtLeast20Age()
        {
            return "At least 20 age";
        }

        [HttpGet("GetForAtLeast25Age")]
        [MinimumAgeAuthorize(25)]
        public string GetForAtLeast25Age()
        {
            return "At least 25 age";
        }
    }
}
