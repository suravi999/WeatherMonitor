using Microsoft.AspNetCore.Mvc;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Application.Interfaces;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherObservationController : ControllerBase
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

        // POST method to get specific weather data pieces
        [HttpPost("{wmoId}")]
        public async Task<ActionResult<List<Dictionary<string, object?>>>> GetSpecificWeatherData( string wmoId, [FromBody] WeatherObservationSpecificDataRequest? request = null)
        {
            try
            {
                // If no body provided, use default request
                request ??= new WeatherObservationSpecificDataRequest();

                var data = await _weatherObservationService.GetSpecificWeatherDataAsync(wmoId, request);
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

        // POST method to get weather data with optional request body
        [HttpPost("{wmoId}/summary")]
        public async Task<ActionResult<WeatherObservationSummaryDataResponse>> GetWeatherData(string wmoId, [FromBody] WeatherObservationSummaryDataRequest? request = null)
        {
            try
            {
                //If no body provided, use default request
                request ??= new WeatherObservationSummaryDataRequest();

                var data = await _weatherObservationService.GetWeatherDataAsync(wmoId, request);
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
                var (averageTemp, resCount) = await _weatherObservationService.CalculateAverageTemperatureAsync(wmoId, hours);
                return Ok(new { wmoId, hours, averageTemperature = averageTemp, resultCount = resCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("stations/search/{searchTerm}")]
        public async Task<ActionResult<WeatherObservationStation>> FindStation(string searchTerm)
        {
            try
            {
                var station = await _weatherObservationService.FindStationAsync(searchTerm);
                if (station == null)
                    return NotFound(new { error = $"No station found for search term: {searchTerm}" });

                return Ok(station);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
