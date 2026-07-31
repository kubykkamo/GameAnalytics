using GameAnalytics.Exceptions;
using GameAnalytics.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace GameAnalytics.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync
            (
                HttpContext httpcontext,
                Exception exception,
                CancellationToken cancellationToken

            )
        {
            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError,

            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Something went wrong",
                Detail = $"{exception.Message}",
            };

            httpcontext.Response.StatusCode = statusCode;
            await httpcontext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;

        }


    }

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
