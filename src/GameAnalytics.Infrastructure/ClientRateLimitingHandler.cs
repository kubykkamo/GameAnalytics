using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;
using GameAnalytics.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

public class ClientRateLimitingHandler : DelegatingHandler
{
    private readonly RateLimiter _limiter;
    private readonly ILogger<ClientRateLimitingHandler> _logger;

    public ClientRateLimitingHandler(RateLimiter limiter, ILogger<ClientRateLimitingHandler> logger)
    {
        _limiter = limiter;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Request waiting for rate limiter: {Method} {Uri}",
            request.Method,
            request.RequestUri);

        using var lease = await _limiter.AcquireAsync(1, cancellationToken);

        sw.Stop();

        if (sw.ElapsedMilliseconds > 50)
        {
            _logger.LogInformation(
                "Request waited in rate limiter queue for {ElapsedMs} ms: {Method} {Uri}",
                sw.ElapsedMilliseconds,
                request.Method,
                request.RequestUri);
        }

        if (!lease.IsAcquired)
        {
            _logger.LogWarning("Request rejected - queue full");

            return new HttpResponseMessage(
                HttpStatusCode.TooManyRequests);
        }

        _logger.LogInformation(
            "Request allowed through: {Method} {Uri}",
            request.Method,
            request.RequestUri);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            foreach (var header in response.Headers)
            {
                _logger.LogWarning(
                    "429 header {Key}: {Value}",
                    header.Key,
                    string.Join(", ", header.Value));
            }

            foreach (var header in response.Content.Headers)
            {
                _logger.LogWarning(
                    "429 content header {Key}: {Value}",
                    header.Key,
                    string.Join(", ", header.Value));
            }

            _logger.LogWarning(
                "HenrikDev returned 429 for {Method} {Uri}",
                request.Method,
                request.RequestUri);
        }

        return response;
    }
}