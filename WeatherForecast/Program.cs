
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherForecast.HttpHandlers;
using WeatherForecast.Interfaces;
using WeatherForecast.Services;
using HttpClientHandler = WeatherForecast.HttpHandlers.HttpClientHandler;


var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
var config = builder.Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices(config))
    .Build();

var hostedService = host.Services.GetRequiredService<HostedService>();
await hostedService.ExecuteAsync();

static Action<IServiceCollection> ConfigureServices(IConfiguration config)
{
    return services =>
    {
        // Add the WeatherRequestHandler
        services.AddHttpClient("WeaterService",
            httpClient =>
            {
                httpClient.BaseAddress = new Uri(config["WeatherService:APIBaseAddress"]);
            })
            .AddHttpMessageHandler<WeatherRequestHandler>();

        // Register services
        services.AddSingleton<IConfiguration>(config);
        services.AddTransient<WeatherRequestHandler>();
        services.AddTransient<IHttpClientHandler, HttpClientHandler>();
        services.AddTransient<HostedService>();
    };
}