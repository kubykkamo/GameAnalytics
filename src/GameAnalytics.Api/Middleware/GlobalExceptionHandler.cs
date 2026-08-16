using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GameAnalytics.Domain.Exceptions;
namespace GameAnalytics.Middleware{
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

            var errorTitle = exception switch
            {
                NotFoundException => "Not Found",
                _ => "Something went wrong"
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = errorTitle,
                Detail = $"{exception.Message}",
            };

            httpcontext.Response.StatusCode = statusCode;
            await httpcontext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;

        }


    }
}