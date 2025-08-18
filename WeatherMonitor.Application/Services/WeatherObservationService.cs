using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Application.Interfaces;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Application.Services
{
    public class WeatherObservationService : IWeatherObservationService
    {
        private readonly IWeatherObservationDataRepository _weatherObservationDataRepository; 
        private readonly IWeatherStationRepository _weatherStationRepository;
        public WeatherObservationService(IWeatherObservationDataRepository weatherObservationDataRepository, IWeatherStationRepository weatherStationRepository)
        {
            _weatherObservationDataRepository = weatherObservationDataRepository;
            _weatherStationRepository = weatherStationRepository;
        }

        public async Task<List<WeatherObservationDto>> GetAllWeatherDataAsync(string wmoId)
        {
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(wmoId);
            if (station == null)
            {
                throw new ArgumentException($"Weather station with WMO ID '{wmoId}' not found", nameof(wmoId));
            }

            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);

            return observations.Select(o => new WeatherObservationDto
            {
                StationName = o.StationName,
                WmoId = o.WmoId,
                ObservationTime = o.ObservationTime,
                Temperature = o.Temperature,
                ApparentTemperature = o.ApparentTemperature,
                DewPoint = o.DewPoint,
                Humidity = o.Humidity,
                WindDirection = o.WindDirection,
                WindSpeed = o.WindSpeed,
                Pressure = o.Pressure,
                WeatherCondition = o.WeatherCondition
            }).ToList();
        }

        public async Task<List<Dictionary<string, object?>>> GetSpecificWeatherDataAsync(string wmoId, WeatherObservationSpecificDataRequest request)
        {
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(wmoId);
            if (station == null)
            {
                throw new ArgumentException($"Weather station with WMO ID '{wmoId}' not found", nameof(wmoId));
            }

            // Get observations
            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);

            // Create simple array of data objects
            var result = new List<Dictionary<string, object?>>();

            foreach (var observation in observations)
            {
                var dataObject = new Dictionary<string, object?>();

                // If no specific fields requested, include all data
                if (request.RequestedDataTypes == null || !request.RequestedDataTypes.Any())
                {
                    dataObject = new Dictionary<string, object?>
                    {
                        ["temperature"] = observation.Temperature,
                        ["apparentTemperature"] = observation.ApparentTemperature,
                        ["dewPoint"] = observation.DewPoint,
                        ["humidity"] = observation.Humidity,
                        ["windDirection"] = observation.WindDirection,
                        ["windSpeed"] = observation.WindSpeed,
                        ["pressure"] = observation.Pressure,
                        ["weatherCondition"] = observation.WeatherCondition,
                        ["observationTime"] = observation.ObservationTime
                    };
                }
                else
                {
                    // Include only requested fields
                    foreach (var fieldName in request.RequestedDataTypes)
                    {
                        var value = GetFieldValue(observation, fieldName.ToLower());
                        if (value != null)
                        {
                            dataObject[fieldName] = value;
                        }
                    }
                }

                // Only add if we have data
                if (dataObject.Any())
                {
                    result.Add(dataObject);
                }
            }

            return result;
        }

        public async Task<WeatherObservationStation?> FindStationAsync(string searchTerm)
        {
            //Try search using WMO ID first
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(searchTerm);
            if (station != null) return station;

            //then try using station name
            return await _weatherStationRepository.GetStationByNameAsync(searchTerm);
        }

        public async Task<WeatherObservationSummaryDto> GetWeatherSummaryAsync(string wmoId)
        {
            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(wmoId);

            var (averageTemp, recCount) = await CalculateAverageTemperatureAsync(wmoId);

            return new WeatherObservationSummaryDto
            {
                StationName = station?.Name ?? "Unknown Station",
                WmoId = wmoId,
                AverageTemperature = averageTemp,
                ObservationCount = recCount
            };
        }

        public async Task<(double AverageTemperature, int RecordCount)> CalculateAverageTemperatureAsync(string wmoId, int hours = 72)
        {
            if (hours <= 0)
            {
                hours = 72; //Default to 72 hours if invalid input
            }
            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);
            var cutoffTime = DateTime.UtcNow.AddHours(-hours); //Calculate the cutoff time form current utc time, since we are dealing with UTC time of the response

            var recentObservations = observations
                .Where(o => o.ObservationTime >= cutoffTime && o.Temperature.HasValue)
                .ToList();

            if (!recentObservations.Any())
                return (0, 0);

            var averageTemp = recentObservations.Average(o => o.Temperature!.Value);
            return (averageTemp, recentObservations.Count);

        }

        public async Task<List<WeatherObservationStation>> GetAvailableStationsAsync()
        {
            return await _weatherStationRepository.GetAllStationsAsync();
        }

        public async Task<WeatherObservationSummaryDataResponse> GetWeatherDataAsync(string wmoId, WeatherObservationSummaryDataRequest request)
        {
            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);
            var latest = observations.FirstOrDefault();//assuming the first one is the latest
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(wmoId);

            var (averageTemp, recCount) = await CalculateAverageTemperatureAsync(wmoId, request.TimeRangeHours);

            var response = new WeatherObservationSummaryDataResponse
            {
                StationName = station?.Name ?? "Unknown Station",
                WmoId = wmoId,
                AverageTemperature = averageTemp,
                ObservationCount = recCount
            };

            //Add requested additional fields
            if (request.AdditionalFields.Any())
            {
                var (additionalData, unavailableFields) = ExtractAdditionalFields(latest, request.AdditionalFields);
                response.AdditionalRequestedFields = additionalData;
                response.UnavailableRequestedFields = unavailableFields;
            }

            return response;
        }

        private (Dictionary<string, object?> data, List<string> unavailable) ExtractAdditionalFields( WeatherObservation observation, List<string> requestedFields)
        {
            var data = new Dictionary<string, object?>();
            var unavailable = new List<string>();

            foreach (var field in requestedFields)
            {
                var value = GetFieldValue(observation, field.ToLower());
                if (value != null)
                {
                    data[field] = value;
                }
                else
                {
                    unavailable.Add(field);
                }
            }

            return (data, unavailable);
        }

        private object? GetFieldValue(WeatherObservation observation, string fieldName)
        {
            return fieldName switch
            {
                "temperature" or "temp" => observation.Temperature,
                "apparenttemperature" or "apptemp" or "apparent" => observation.ApparentTemperature,
                "dewpoint" or "dew" => observation.DewPoint,
                "humidity" or "humid" => observation.Humidity,
                "winddirection" or "winddir" => observation.WindDirection,
                "windspeed" or "wind" => observation.WindSpeed,
                "pressure" or "press" => observation.Pressure,
                "condition" or "weather" => observation.WeatherCondition,
                "observationtime" or "time" => observation.ObservationTime,
                _ => null
            };
        }

        
    }
}
