using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Application.Extensions;
using Application.Services.Core;

namespace API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .SelectMany(ms => ms.Value.Errors.Select(e => new string(e.ErrorMessage)).ToList());

                var result = ApiResult<string>.ERROR(new Error
                {
                    HttpStatus = System.Net.HttpStatusCode.BadRequest,
                    ErrorCode = CustomExceptionCodes.ValidationException,
                    ErrorMessage = CustomExceptionCodes.ValidationException.GetEnumDescription(),
                    ValidationErrors = errors.ToList()
                });

                context.Result = new BadRequestObjectResult(result);
            }
        }
    }
}