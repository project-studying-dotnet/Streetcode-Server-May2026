using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Streetcode.Shared.Web.Middleware
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, string, long, int, Exception?> _requestFinished =
            LoggerMessage.Define<string, string, long, int>(
                LogLevel.Information,
                new EventId(1, "RequestFinished"),
                "Finished {Method} {Path} in {Ms}ms with status {Status}");

        public static void LogRequestFinished(this ILogger logger, string method, string path, long ms, int status)
        {
            _requestFinished(logger, method, path, ms, status, null);
        }
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                _logger.LogRequestFinished(
                    context.Request.Method,
                    context.Request.Path,
                    sw.ElapsedMilliseconds,
                    context.Response.StatusCode);
            }
        }
    }
}