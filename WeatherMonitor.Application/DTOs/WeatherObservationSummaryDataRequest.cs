using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Application.DTOs
{
    public class WeatherObservationSummaryDataRequest
    {
        public List<string> AdditionalFields { get; set; } = new();
        public int TimeRangeHours { get; set; } = 72;
    }
}
