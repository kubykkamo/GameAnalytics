
using Microsoft.Extensions.Logging;
using GameAnalytics.Domain.Exceptions;
namespace GameAnalytics.Infrastructure
{

    public class ExternalApiErrorHandler : DelegatingHandler
    {

        private readonly ILogger<ExternalApiErrorHandler> _logger;


        public ExternalApiErrorHandler(ILogger<ExternalApiErrorHandler> logger)
        { 
            _logger = logger;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)


      
        {

            var response = await base.SendAsync(request, cancellationToken);


            if (response.IsSuccessStatusCode)
            {
                return response;
            }


            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var failedUrl = request.RequestUri?.ToString();

            _logger.LogWarning("Henrik API request failed. Status: {StatusCode} | URL: {Url} | Response: {ErrorContent}",
            (int)response.StatusCode,
            failedUrl,
            errorContent);


            throw response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound =>
                    new NotFoundException($"Henrik API 404 at [{failedUrl}]. Details: {errorContent}"),

                System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden =>
                    new UnauthorizedAccessException($"Henrik API Auth Error at [{failedUrl}]. Key invalid."),

                System.Net.HttpStatusCode.TooManyRequests =>
                    new HttpRequestException("Rate limit exceeded on Henrik API."),

                _ => new HttpRequestException($"Henrik API error {(int)response.StatusCode} at [{failedUrl}]. Details: {errorContent}")
            };
        }
    }
}
