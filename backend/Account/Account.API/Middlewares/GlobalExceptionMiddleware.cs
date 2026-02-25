using System.Net;
using Account.Application.Common.Errors;
using Account.Application.Exceptions;

namespace Account.API.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate next,
                                              ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomValidationException ex)
        {
            await HandleCustomValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleInternalExceptionAsync(context, ex);
        }
    }

    private async Task HandleCustomValidationException(HttpContext context, CustomValidationException exception)
    {
        _logger.LogWarning(exception, "Validation failed");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        
        await context.Response.WriteAsJsonAsync(exception.Error);
    }

    private async Task HandleInternalExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogCritical(exception, "Unexpected exception occurred");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var error = Error.Failure(
            ErrorCodes.InternalError,
            ErrorMessages.InternalError
        );

        await context.Response.WriteAsJsonAsync(error);
    }
}
