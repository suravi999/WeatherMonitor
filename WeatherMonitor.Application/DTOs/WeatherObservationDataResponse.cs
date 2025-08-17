using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Application.DTOs
{
    public class WeatherObservationDataResponse : WeatherObservationSummaryDto
    {
        public Dictionary<string, object?> AdditionalRequestedFields { get; set; } = new();
        public List<string> UnavailableRequestedFields { get; set; } = new();

    }
}
