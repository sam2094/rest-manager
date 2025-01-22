using Domain.Enums;
using System.Net;

namespace Application.Services.Core
{
    public sealed class Error
    {
        public HttpStatusCode HttpStatus { get; set; }
        public CustomExceptionCodes ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> ValidationErrors { get; set; }
    }

    public class ApiResult<TResponse>
    {
        public TResponse Result { get; private set; }
        public Error Error { get; set; }
        public bool IsSuccess => Error is null;
        public static ApiResult<TResponse> OK(TResponse response) => new ApiResult<TResponse> { Result = response };
        public static ApiResult<TResponse> ERROR(Error error) => new ApiResult<TResponse> { Error = error, Result = default };
    }
}