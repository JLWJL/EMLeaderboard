using EMLeaderboard.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EMLeaderboard.Common.ExceptionHandlers;

public class GlobalExceptionHandler: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            NotFoundException notFoundException => new ProblemDetails{
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource Not Found",
                Detail = notFoundException.Message,
                Status = StatusCodes.Status404NotFound,
            },
            
            ArgumentException argEx => new ProblemDetails
            {
                Type = "ValidationError",
                Title = "Invalid Input",
                Detail = argEx.Message,
                Status = StatusCodes.Status400BadRequest,
            },

            _ => new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Title = "An unexpected error occurred",
                Detail = "Please try again later",
                Status = StatusCodes.Status500InternalServerError,
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}