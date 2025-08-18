# Weather Monitor System

A professional weather monitoring application built for the Business Information Systems (BIS) team to retrieve and analyze South Australian weather observation data from the Bureau of Meteorology.

## Overview

This Weather Monitor system demonstrates Clean Architecture principles with a clear separation of concerns across four distinct layers. The solution retrieves real-time weather observation data from the Australian Bureau of Meteorology's JSON feeds, calculates 72-hour temperature averages, and provides flexible data access through both Console UI and RESTful Web API interfaces.

## Architecture

- **Domain Layer**: Core business entities (WeatherObservation, WeatherStation)
- **Application Layer**: Business logic and use cases (WeatherService)
- **Infrastructure Layer**: External integrations (Bureau of Meteorology API)
- **Presentation Layer**: Console UI and RESTful Web API

## Features

- Real-time weather data from Bureau of Meteorology
- 72-hour temperature averaging with UTC time calculations
- Console application with intelligent station search
- RESTful Web API with Swagger documentation
- Support for all South Australian weather stations
- Flexible data retrieval (all fields or specific data types)
- User-friendly station lookup (by name or WMO ID)
- Enterprise-ready with comprehensive error handling

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022

### Clone and Build
```bash
git clone <repository-url>
cd WeatherMonitor
dotnet restore
dotnet build
```

## Running the Applications

### Console Application
```bash
# Default (Adelaide Airport)
dotnet run --project .\WeatherMonitor.UI.ConsoleApp

# Specific station by WMO ID
dotnet run --project .\WeatherMonitor.UI.ConsoleApp 94672

# Specific station by name
dotnet run --project .\WeatherMonitor.UI.ConsoleApp "Adelaide Airport"

# With additional weather data
dotnet run --project .\WeatherMonitor.UI.ConsoleApp 94672 --detailed
dotnet run --project .\WeatherMonitor.UI.ConsoleApp 94672 --temperature --humidity
```

### Web API
```bash
# Start the API server (IIS).

# API will be available at:
# https://localhost:44303


# Swagger UI available at:
# https://localhost:44303/swagger/index.html
```

## Project Structure

```
WeatherMonitor/
├── src/
│   ├── WeatherMonitor.Core/               # Domain Project. Core entities and interfaces
│   ├── WeatherMonitor.Application/        # Business logic and DTOs
│   ├── WeatherMonitor.Infrastructure/     # External integrations
│   ├── WeatherMonitor.WebApi/            # REST API endpoints
│   └── WeatherMonitor.UI.ConsoleApp/         # Console interface
├── tests/
│   └── WeatherMonitor.WebApi.Tests/      # API unit tests
└── README.md
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/WeatherMonitor.WebApi.Tests

# Run tests with detailed output
dotnet test --verbosity normal
```

### Visual Studio Testing
1. Open **Test Explorer** (Ctrl+E, T)
2. Build solution (Ctrl+Shift+B)
3. Click **"Run All Tests"** or press Ctrl+R, A

## Available Weather Stations

| Station Name | WMO ID | Location |
|--------------|--------|----------|
| Adelaide Airport | 94672 | Metropolitan Adelaide |
| Edinburgh | 95676 | Northern Adelaide (RAAF Base) |
| Hindmarsh Island | 94677 | Murray River mouth region |
| Kuitpo | 94683 | Adelaide Hills region |

