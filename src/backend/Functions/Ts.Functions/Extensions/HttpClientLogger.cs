using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;

namespace TelemetryStash.Functions.Extensions;

public static class HttpClientLoggerExtensions
{
    public static IServiceCollection AddHttpClientLogger(this IServiceCollection services)
    {
        services.AddSingleton<HttpClientLogger>();

        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.RemoveAllLoggers();
            builder.AddLogger<HttpClientLogger>();
        });

        return services;
    }
}

internal class HttpClientLogger(ILogger<HttpClientLogger> logger) : IHttpClientLogger
{
    private record Context(string? OriginalRequestUrl);

    private readonly ILogger<HttpClientLogger> _logger = logger;

    public object? LogRequestStart(HttpRequestMessage request)
    {
        // Store the original request url, redirects will overwrite the url in HttpRequestMessage
        return new Context(request.RequestUri?.AbsoluteUri);
    }

    public void LogRequestStop(object? context, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed)
    {
        _logger.LogInformation(
            "{Method} {Url} - {(StatusCode} {StatusCodeDescription} in {Duration}ms",
            request.Method,
            ((Context)context!).OriginalRequestUrl,
            (int)response.StatusCode,
            response.StatusCode,
            elapsed.TotalMilliseconds);
    }

    public void LogRequestFailed(object? context, HttpRequestMessage request, HttpResponseMessage? response, Exception exception, TimeSpan elapsed)
    {
        _logger.LogError(
            exception,
            "{Method} {Url} - Exception",
            request.Method,
            ((Context)context!).OriginalRequestUrl);
    }
}
