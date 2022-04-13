using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XXTk.Auth.Samples.RoleBased.HttpApi.Controllers
{
    /// <summary>
    /// Authorize 默认要求通过身份认证的用户才可访问
    /// </summary>
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

        /// <summary>
        /// AllowAnonymous 允许匿名访问
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 必须拥有角色“Admin”才可访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForAdmin")]
        [Authorize(Roles = "Admin")]
        public string GetForAdmin()
        {
            return "Admin only";
        }

        /// <summary>
        /// 必须拥有角色“Developer” 或 “Tester”才可访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForDeveloperOrTester")]
        [Authorize(Roles = "Developer,Tester")]
        public string GetForDeveloperOrTester()
        {
            return "Developer || Tester";
        }

        /// <summary>
        /// 必须拥有角色“Developer” 和 “Tester”才可访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForDeveloperAndTester")]
        [Authorize(Roles = "Developer")]
        [Authorize(Roles = "Tester")]
        public string GetForDeveloperAndTester()
        {
            return "Developer && Tester";
        }
    }
}
