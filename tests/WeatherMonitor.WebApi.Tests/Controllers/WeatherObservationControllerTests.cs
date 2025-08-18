using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Application.Interfaces;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.WebApi.Controllers;

namespace WeatherMonitor.WebApi.Tests.Controllers
{
    public class WeatherObservationControllerTests
    {
        private readonly Mock<IWeatherObservationService> _mockService;
        private readonly WeatherObservationController _controller;
        public WeatherObservationControllerTests()
        {
            _mockService = new Mock<IWeatherObservationService>();
            _controller = new WeatherObservationController(_mockService.Object);
        }

        [Fact]
        public async Task GetWeatherData_WithValidWmoId_ReturnsOk()
        {
            // Arrange
            var wmoId = "94672";
            var expectedData = new List<WeatherObservationDto>
            {
                new WeatherObservationDto
                {
                    StationName = "Adelaide Airport",
                    WmoId = wmoId
                }
            };

            _mockService.Setup(x => x.GetAllWeatherDataAsync(wmoId))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetWeatherData(wmoId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetWeatherData_WithInvalidWmoId_ReturnsNotFound()
        {
            // Arrange
            var wmoId = "invalid Station";
            _mockService.Setup(x => x.GetAllWeatherDataAsync(wmoId))
                .ThrowsAsync(new ArgumentException("Station not found"));

            // Act
            var result = await _controller.GetWeatherData(wmoId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetSpecificWeatherData_WithRequest_ReturnsOk()
        {
            // Arrange
            var wmoId = "94672";
            var request = new WeatherObservationSpecificDataRequest
            {
                RequestedDataTypes = new List<string> { "temperature", "humidity" }
            };
            var expectedData = new List<Dictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["temperature"] = 10.0, ["humidity"] = 65.0 }
            };

            _mockService.Setup(x => x.GetSpecificWeatherDataAsync(wmoId, request))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetSpecificWeatherData(wmoId, request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetSpecificWeatherData_WithNullRequest_ReturnsOk()
        {
            // Arrange
            var wmoId = "94672";
            var expectedData = new List<Dictionary<string, object?>>();

            _mockService.Setup(x => x.GetSpecificWeatherDataAsync(wmoId, It.IsAny<WeatherObservationSpecificDataRequest>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetSpecificWeatherData(wmoId, null);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetWeatherSummary_WithValidWmoId_ReturnsOk()
        {
            // Arrange
            var wmoId = "94672";
            var expectedSummary = new WeatherObservationSummaryDto
            {
                StationName = "Adelaide Airport",
                WmoId = wmoId,
                AverageTemperature = 10,
                ObservationCount = 50
            };

            _mockService.Setup(x => x.GetWeatherSummaryAsync(wmoId))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _controller.GetWeatherSummary(wmoId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedSummary);

            // Additional assertion for AverageTemperature
            var summary = okResult.Value as WeatherObservationSummaryDto;
            summary.Should().NotBeNull();
            summary!.AverageTemperature.Should().NotBe(null);
        }

        [Fact]
        public async Task GetStations_ReturnsOkWithStations()
        {
            // Arrange
            var expectedStations = new List<WeatherObservationStation>
            {
                new WeatherObservationStation { Name = "Adelaide Airport", WmoId = "94672" },
                new WeatherObservationStation { Name = "Edinburgh", WmoId = "95676" }
            };

            _mockService.Setup(x => x.GetAvailableStationsAsync())
                .ReturnsAsync(expectedStations);

            // Act
            var result = await _controller.GetStations();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedStations);
        }

        [Fact]
        public async Task GetAverageTemperature_WithValidWmoId_ReturnsOk()
        {
            // Arrange
            var wmoId = "94672";
            var hours = 72;
            var expectedTemp = 24.5;
            var expectedCount = 48;

            _mockService.Setup(x => x.CalculateAverageTemperatureAsync(wmoId, hours))
                .ReturnsAsync((expectedTemp, expectedCount));

            // Act
            var result = await _controller.GetAverageTemperature(wmoId, hours);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task FindStation_WithValidSearchTerm_ReturnsOk()
        {
            // Arrange
            var searchTerm = "Adelaide";
            var expectedStation = new WeatherObservationStation
            {
                Name = "Adelaide Airport",
                WmoId = "94672"

            };

            _mockService.Setup(x => x.FindStationAsync(searchTerm))
                .ReturnsAsync(expectedStation);

            // Act
            var result = await _controller.FindStation(searchTerm);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedStation);
        }

        [Fact]
        public async Task FindStation_WithInvalidSearchTerm_ReturnsNotFound()
        {
            // Arrange
            var searchTerm = "NonExistent";
            _mockService.Setup(x => x.FindStationAsync(searchTerm))
                .ReturnsAsync((WeatherObservationStation?)null);

            // Act
            var result = await _controller.FindStation(searchTerm);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetWeatherData_WhenServiceThrows_ReturnsBadRequest()
        {
            // Arrange
            var wmoId = "94672";
            _mockService.Setup(x => x.GetAllWeatherDataAsync(wmoId))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.GetWeatherData(wmoId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
