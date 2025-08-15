using Microsoft.AspNetCore.Mvc;
using WeatherMonitor.Application.Interfaces;

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
    }
}
