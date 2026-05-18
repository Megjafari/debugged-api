using System.Text.Json;
using Debugged.Application.Common.Exceptions;

namespace Debugged.API.Middleware;

// Catches unhandled exceptions and converts them into RFC 7807 ProblemDetails responses.
// Keeps controllers clean — they never need try/catch for domain exceptions.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Map known domain exceptions to HTTP responses. Unknown ones become 500.
        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "One or more validation errors occurred.",
                (IDictionary<string, string[]>?)validationEx.Errors),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                notFoundEx.Message,
                null),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                // Only leak exception details in Development — production gets a generic message.
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                null)
        };

        // 500-level errors are unexpected — log them. Client errors (4xx) are noise at Error level.
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        // ProblemDetails shape follows RFC 7807 — same format ASP.NET Core uses for built-in 400s.
        var problemDetails = new
        {
            type = $"https://httpstatuses.io/{statusCode}",
            title,
            status = statusCode,
            detail,
            instance = context.Request.Path.Value,
            errors
        };

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}