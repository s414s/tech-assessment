
using BookingSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookingSystem.Presentation.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        var details = new ProblemDetails()
        {
            Title = title,
            Status = statusCode,
            Detail = "API Error",
            Instance = $"API {httpContext.Request.Path}",
            Type = MapType(statusCode),
            Extensions = MapExtensions(exception, traceId)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                    "Exception on machine {MachineName}. TraceId: {TraceId}. Msg: {Ex}",
                    Environment.MachineName,
                    traceId,
                    exception.Message);
        }

        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException => (StatusCodes.Status400BadRequest, exception.Message),
            NotImplementedException => (StatusCodes.Status400BadRequest, "Not Implemented"),
            ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };
    }

    private static Dictionary<string, object?> MapExtensions(Exception exception, string? traceId)
    {
        Dictionary<string, object?> result = [];

        if (traceId is string tId)
        {
            result.Add("traceId", tId);
        }

        return result;
    }

    private static string MapType(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            StatusCodes.Status500InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }
}

