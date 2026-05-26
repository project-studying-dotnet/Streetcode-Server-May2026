using System.Net;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Exceptions;

namespace Streetcode.WebApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly bool _showDetails;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _showDetails = bool.TryParse(config["ShowExceptionDetails"], out var result) && result;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request {Path}", context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString()
                            ?? context.TraceIdentifier;

        context.Response.ContentType = "application/json";
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var problem = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case NotFoundException nf:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                problem.Status = (int)HttpStatusCode.NotFound;
                problem.Title = "Not Found";
                problem.Detail = nf.Message;
                break;

            case ForbiddenException:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                problem.Status = (int)HttpStatusCode.Forbidden;
                problem.Title = "Forbidden";
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                problem.Status = (int)HttpStatusCode.Unauthorized;
                problem.Title = "Unauthorized";
                break;

            case BaseException baseEx:
                context.Response.StatusCode = baseEx.StatusCode;
                problem.Status = baseEx.StatusCode;
                problem.Title = "Business Logic Error";
                problem.Detail = baseEx.Message;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problem.Status = (int)HttpStatusCode.InternalServerError;
                problem.Title = "Server error";
                problem.Detail = _showDetails
                    ? exception.Message
                    : "An internal error occurred.";
                break;
        }

        problem.Extensions["correlationId"] = correlationId;

        await context.Response.WriteAsJsonAsync(problem);
    }
}