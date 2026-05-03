# API Aggregator

A .NET Web API that aggregates data from multiple external APIs (GitHub, Weather, and News) into a single unified response.

## Features

- Aggregates data from multiple external APIs
- Executes API calls in parallel for better performance
- Uses per-API caching with IMemoryCache
- Handles failures gracefully (fallback mechanism)
- Tracks request statistics per external API
- Supports filtering and sorting
- Includes unit tests for core functionality

## Endpoints

### GET /api/aggregation
```http
GET /api/aggregation
```
Aggregates data from GitHub, Weather, and News APIs.

#### Query Parameters

- `searchQuery` (string, required): Search term for GitHub and News
- `city` (string, optional): City for weather data. Defaults to Athens
- `sortBy` (enum, optional): Sorting for news. Supported values: Date, Relevance, Popularity

### Example Request
```http
GET /api/aggregation?searchQuery=dotnet&city=athens&sortBy=Date
```

### Example Response
```json
{
  "repositories": [
    {
      "name": "aspnetcore",
      "url": "https://github.com/dotnet/aspnetcore",
      "stars": 100
    }
  ],
  "weather": {
    "city": "Athens",
    "temperature": 20,
    "description": "clear sky"
  },
  "news": [
    {
      "title": "Dotnet news",
      "source": "Test Source",
      "url": "https://example.com",
      "publishedAt": "2026-01-01T00:00:00Z"
    }
  ],
  "errors": []
}
```

### GET /api/stats
```http
GET /api/stats
```

Returns request statistics per external API.

### Example Response
```json
{
  "apiStats": {
    "GitHub": {
      "totalRequests": 3,
      "averageResponseTimeMs": 120,
      "fastRequests": 2,
      "averageRequests": 1,
      "slowRequests": 0
    }
  }
}
```
## Setup & Configuration

### Requirements
.NET 8 SDK


### API Keys

Add your API keys in appsettings.Development.json:
```json
{
  "ExternalApis": {
    "OpenWeather": {
      "ApiKey": "OPEN_WEATHER_API_KEY"
    },
    "NewsApi": {
      "ApiKey": "NEWS_API_KEY"
    }
  }
}
```