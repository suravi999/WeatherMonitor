using System.Text.Json;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.ConsoleApp
{
    class Program
    {
        private const string DefaultWmoId = "94672"; // Adelaide Airport by default
        private const string API_BASE_WEATHER = "https://localhost:44303"; // Update this to your actual API base URL, if profile https use "https://localhost:7131"

        private static readonly HttpClient _httpClient = new();
        private static string _apiWeatherUrl = API_BASE_WEATHER + "/api/WeatherObservation";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Weather Monitor Console Application ===\n");

            if(args.Length == 1 && (args[0].ToLower() == "help" || args[0].ToLower() == "-h"))
            {
                await Help();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                string wmoId = await GetWmoIdFromArgs(args);
                await DisplayWeatherSummary(wmoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task<string> GetWmoIdFromArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No WMO ID provided, using default: Adelaide Airport (94672)\n");
                return DefaultWmoId;
            }

            string input = args[0];

            //Try to find station by searching via API
            try
            {
                var response = await _httpClient.GetAsync($"{_apiWeatherUrl}/stations/search/{input}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var station = JsonSerializer.Deserialize<WeatherObservationStation>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (station != null)
                    {
                        Console.WriteLine($"Found station: {station.Name} (WMO: {station.WmoId})\n");
                        return station.WmoId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API search failed: {ex.Message}");
            }

            //If not found, assume it is a WMO ID
            Console.WriteLine($"Using WMO ID: {input}\n");
            return input;
        }

        private static async Task DisplayWeatherSummary(string wmoId)
        {
            Console.WriteLine("Retrieving weather data from API...\n");

            try
            {
                //Call the API to get weather summary
                var summaryResponse = await _httpClient.GetAsync($"{_apiWeatherUrl}/{wmoId}/summary");

                if (!summaryResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error calling API: {summaryResponse.StatusCode}");
                    return;
                }

                var summaryJson = await summaryResponse.Content.ReadAsStringAsync();
                var summary = JsonSerializer.Deserialize<WeatherObservationSummaryDto>(summaryJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (summary == null)
                {
                    Console.WriteLine("Failed to parse weather summary");
                    return;
                }

                Console.WriteLine("*** Weather Station Summary ***");
                Console.WriteLine($"Station Name: {summary.StationName}");
                Console.WriteLine($"WMO ID: {summary.WmoId}");
                Console.WriteLine($"Average Temperature (72 hours): {summary.AverageTemperature:F1}°C");
                Console.WriteLine($"Number of Observations: {summary.ObservationCount}");

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error calling API: {ex.Message}");
                Console.WriteLine($"Make sure the WebAPI is running on {API_BASE_WEATHER}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task Help() 
        {
            Console.WriteLine("\nUsage: WeatherMonitor.ConsoleApp.exe [WMO_ID or Station_Name]");
            Console.WriteLine("Example: WeatherMonitor.ConsoleApp.exe 94672");
            Console.WriteLine("Example: WeatherMonitor.ConsoleApp.exe \"Adelaide Airport\"");

            //Display available stations by calling API
            await DisplayAvailableStations();
        }

        private static async Task DisplayAvailableStations()
        {
            try
            {
                Console.WriteLine("\n=== Available Weather Stations ===");

                var stationsResponse = await _httpClient.GetAsync($"{_apiWeatherUrl}/stations");
                if (stationsResponse.IsSuccessStatusCode)
                {
                    var stationsJson = await stationsResponse.Content.ReadAsStringAsync();
                    var stations = JsonSerializer.Deserialize<List<WeatherObservationStation>>(stationsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (stations != null)
                    {
                        foreach (var station in stations)
                        {
                            Console.WriteLine($"• {station.Name} (WMO: {station.WmoId})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retrieve stations: {ex.Message}");
            }
        }
    }
}
