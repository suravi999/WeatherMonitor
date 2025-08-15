using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Application.DTOs
{
    public class WeatherObservationSummaryDto
    {
        public string WmoId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public DateTime LastObservationTime { get; set; }
        public double AverageTemperature { get; set; }
        public int ObservationCount { get; set; }
    }
}
