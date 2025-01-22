using Microsoft.AspNetCore.Http;

namespace Application.Extension
{
    public static class HttpContextExtension
    {
        public static async Task<string> GetBodyAsStringAsync(this HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            httpContext.Request.Body.Position = 0;
            string body = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
            httpContext.Request.Body.Position = 0;

            return body;
        }

        public static async Task<string> GetResponseBodyAsStringAsync(this HttpContext httpContext, Func<Task> action)
        {
            Stream originalBody = httpContext.Response.Body;
            using MemoryStream memoryStream = new();
            httpContext.Response.Body = memoryStream;

            await action();

            httpContext.Request.EnableBuffering();
            memoryStream.Position = 0;
            string responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBody);
            httpContext.Response.Body = originalBody;

            return responseBody;
        }
    }
}