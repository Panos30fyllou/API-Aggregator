using ApiAggregator.Interfaces;
using ApiAggregator.Services;
using ApiAggregator.Services.ExternalApis;

namespace ApiAggregator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAggregationService, AggregationService>();

        services.AddHttpClient<IGitHubService, GitHubService>(client => client.DefaultRequestHeaders.UserAgent.ParseAdd("ApiAggregator/1.0"));
        services.AddHttpClient<INewsService, NewsService>(client => client.DefaultRequestHeaders.UserAgent.ParseAdd("ApiAggregator/1.0"));
        services.AddHttpClient<IWeatherService, WeatherService>();

        return services;
    }
}