using Application.Services.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Extension
{
    public static class MediatrExtensions
    {
        public async static Task<ActionResult<ApiResult<TResponse>>> HandleRequest<TResponse>(this IMediator mediator, BaseRequest<ApiResult<TResponse>> request, HttpContext httpContext)
        {
            var result = await mediator.Send(request);

            if (result.Error != null)
            {
                httpContext.Response.StatusCode = (int)result.Error.HttpStatus;
                return new ObjectResult(result);
            }
            else
            {
                return new OkObjectResult(result);
            }
        }
    }
}