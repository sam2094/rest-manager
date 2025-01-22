using Application.Extensions;
using Application.Interfaces.Common;
using Application.Services.Core;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Application.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleError(context, exception);
            }
        }

        private async Task HandleError(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;


            var result = ApiResult<string>.ERROR(new Error
            {
                ErrorCode = CustomExceptionCodes.UnHandledException,
                HttpStatus = System.Net.HttpStatusCode.InternalServerError,
                ErrorMessage = CustomExceptionCodes.UnHandledException.GetEnumDescription()
            });

            var innerExceptionMessage = exception.InnerException?.Message;
            var innerStackTrace = exception.InnerException?.StackTrace;

            string errorMessage = $"Custom Exception: \n: {CustomExceptionCodes.UnHandledException} \n" +
                              "Original exception: \n" + exception?.Message + "\n" +
                              "Inner exception: \n" + innerExceptionMessage + "\n" +
                              "Stack Trace: \n" + innerStackTrace + "\n";

            await _logger.LogToConsoleAsync(errorMessage);

            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}