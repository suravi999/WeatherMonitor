using Microsoft.AspNetCore.Mvc;
using WeatherMonitor.Application.Interfaces;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherObservationController: ControllerBase
    {
        private readonly IWeatherObservationService _weatherObservationService;
        public WeatherObservationController(IWeatherObservationService weatherObservationService)
        {
            _weatherObservationService = weatherObservationService;
        }

        [HttpGet("{wmoId}")]
        public async Task<ActionResult> GetWeatherData(string wmoId)
        {
            try
            {
                var data = await _weatherObservationService.GetAllWeatherDataAsync(wmoId);
                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{wmoId}/summary")]
        public async Task<ActionResult> GetWeatherSummary(string wmoId)
        {
            try
            {
                var summary = await _weatherObservationService.GetWeatherSummaryAsync(wmoId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("stations")]
        public async Task<ActionResult<IEnumerable<WeatherObservationStation>>> GetStations()
        {
            try
            {
                var stations = await _weatherObservationService.GetAvailableStationsAsync();
                return Ok(stations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{wmoId}/average-temperature")]
        public async Task<ActionResult> GetAverageTemperature(string wmoId, [FromQuery] int hours = 72)
        {
            try
            {
                var average = await _weatherObservationService.CalculateAverageTemperatureAsync(wmoId, hours);
                return Ok(new { wmoId, hours, averageTemperature = average });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
