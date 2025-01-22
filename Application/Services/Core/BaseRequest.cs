using Application.Interfaces.Common;
using MediatR;

namespace Application.Services.Core
{
    public class BaseRequest<TResponse> : IRequest<TResponse>
    {
        public Error Error { get; set; }
        protected ILogger Logger { get; set; }
    }
}