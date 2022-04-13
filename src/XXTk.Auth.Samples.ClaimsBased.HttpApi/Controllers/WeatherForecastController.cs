using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XXTk.Auth.Samples.ClaimsBased.HttpApi.Controllers
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

        /// <summary>
        /// 仅要求用户具有声明“Rank”，不关心值是多少
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForRankClaim")]
        [Authorize(Policy = "RankClaim")]
        public string GetForRankClaim()
        {
            return "Rank claim only";
        }

        /// <summary>
        /// 要求用户具有声明“Rank”，且值为“P3”
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForRankClaimP3")]
        [Authorize(Policy = "RankClaimP3")]
        public string GetForRankClaimP3()
        {
            return "Rank claim P3";
        }

        /// <summary>
        /// 要求用户具有声明“Rank”，且值为“M3”
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForRankClaimM3")]
        [Authorize(Policy = "RankClaimM3")]
        public string GetForRankClaimM3()
        {
            return "Rank claim M3";
        }

        /// <summary>
        /// 要求用户具有声明“Rank”，且值为“P3” 或 “M3”
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForRankClaimP3OrM3")]
        [Authorize(Policy = "RankClaimP3OrM3")]
        public string GetForRankClaimP3OrM3()
        {
            return "Rank claim P3 || M3";
        }

        /// <summary>
        /// 要求用户具有声明“Rank”，且值为“P3” 和 “M3”
        /// </summary>
        /// <returns></returns>
        [HttpGet("v1/GetForRankClaimP3AndM3")]
        [Authorize(Policy = "RankClaimP3AndM3")]
        public string GetForRankClaimP3AndM3V1()
        {
            return "Rank claim P3 && M3";
        }

        /// <summary>
        /// 要求用户具有声明“Rank”，且值为“P3” 和 “M3”
        /// </summary>
        /// <returns></returns>
        [HttpGet("v2/GetForRankClaimP3AndM3")]
        [Authorize(Policy = "RankClaimP3")]
        [Authorize(Policy = "RankClaimM3")]
        public string GetForRankClaimP3AndM3V2()
        {
            return "Rank claim P3 && M3";
        }
    }
}
