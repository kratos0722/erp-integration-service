using System.Text.Json;
using ErpIntegration.Application.Common;
using FluentValidation;

namespace ErpIntegration.Api.Middleware;

/// <summary>
/// Converts validation and domain exceptions into consistent JSON problem responses,
/// keeping controllers free of try/catch and error-shaping logic.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Validation failed",
                ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
        }
        catch (NotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound, "Resource not found", [ex.Message]);
        }
        catch (BusinessRuleException ex)
        {
            await WriteProblem(context, StatusCodes.Status409Conflict, "Business rule violation", [ex.Message]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);
            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Unexpected error",
                ["An unexpected error occurred."]);
        }
    }

    private static async Task WriteProblem(HttpContext context, int status, string title, IEnumerable<string> errors)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { title, status, errors });
        await context.Response.WriteAsync(payload);
    }
}
