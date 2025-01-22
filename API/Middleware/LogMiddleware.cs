using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Application.Extension;

namespace API.Middleware;

public class LogMiddleware
{
    private readonly RequestDelegate _next;

    public LogMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext httpContext, Application.Interfaces.Common.ILogger logger)
    {
        bool isSwaggerRequest = httpContext.Request.Path.ToString().Contains('.');

        if (!isSwaggerRequest)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("-------------------- Request started --------------------\n");
            stringBuilder.Append($"IP: {httpContext.Connection.RemoteIpAddress?.ToString()}\n");
            stringBuilder.Append($"Date: {DateTime.Now}\n");
            stringBuilder.Append($"Method: {httpContext.Request.Method}\n");
            stringBuilder.Append($"Endpoint: {httpContext.Request.GetEncodedUrl()}\n");

            if (httpContext.Request.Method is "GET")
            {
                stringBuilder.Append($"Params:\n");
                foreach (var keyValue in httpContext.Request.Query)
                    stringBuilder.Append($"{keyValue.Key} = {keyValue.Value}\n");
            }
            else
            {
                stringBuilder.Append($"Body:\n{await httpContext.GetBodyAsStringAsync()}\n");
            }

            await logger.LogToConsoleAsync($"{stringBuilder}");
        }

        string responseBody = await httpContext.GetResponseBodyAsStringAsync(async () =>
        {
            await _next(httpContext);
        });

        if (!isSwaggerRequest)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Response body: {responseBody}");
            stringBuilder.Append("-------------------- Request ended --------------------\n");

            await logger.LogToConsoleAsync($"{stringBuilder}");
        }
    }
}

public static class LogMiddlewareExtensions
{
    public static IApplicationBuilder UseLogging(this IApplicationBuilder builder) => builder.UseMiddleware<LogMiddleware>();
}