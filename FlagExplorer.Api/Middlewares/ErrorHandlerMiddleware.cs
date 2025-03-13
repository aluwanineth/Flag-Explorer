using System.Net;
using System.Text.Json;

namespace FlagExplorer.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case HttpRequestException httpRequestException:
                code = httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new { error = httpRequestException.Message });
                _logger.LogError("HTTP Request error: {Message}, StatusCode: {StatusCode}, Stack Trace: {StackTrace}",
                    httpRequestException.Message,
                    httpRequestException.StatusCode,
                    httpRequestException.StackTrace);
                break;

            case KeyNotFoundException keyNotFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { error = "Resource not found" });
                _logger.LogError("Key not found error: {Message}, Stack Trace: {StackTrace}",
                    keyNotFoundException.Message,
                    keyNotFoundException.StackTrace);
                break;

            case ArgumentException argumentException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = exception.Message });
                _logger.LogError("Argument error: {Message}, Stack Trace: {StackTrace}",
                    argumentException.Message,
                    argumentException.StackTrace);
                break;

            default:
                result = JsonSerializer.Serialize(new { error = "An error occurred" });
                _logger.LogError("Unhandled exception: {Message}, Type: {Type}, Stack Trace: {StackTrace}",
                    exception.Message,
                    exception.GetType().Name,
                    exception.StackTrace);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}