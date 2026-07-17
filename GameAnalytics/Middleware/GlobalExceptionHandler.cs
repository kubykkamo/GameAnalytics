using GameAnalytics.Exceptions;
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
                Detail = exception.Message
            };

            httpcontext.Response.StatusCode = statusCode;
            await httpcontext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
             
        }
    }
}
