using Application.Services.Core;
using Domain.Models;
using MediatR;
using Application.Interfaces;
using Domain.Enums;
using Application.DTOs.Requests;
using Application.Interfaces.Common;
using Application.Extensions;

namespace Application.Services
{
    public class OnLeaveService : BaseRequest<ApiResult<string>>
    {
        public RemoveClientsRequest Request { get; set; }
        public OnLeaveService(RemoveClientsRequest request) => Request = request;

        public class OnLeaveServiceHandler : IRequestHandler<OnLeaveService, ApiResult<string>>
        {
            private readonly IRestManager _restManager;
            private readonly ILogger _logger;

            public OnLeaveServiceHandler(IRestManager restManager, ILogger logger)
            {
                _restManager = restManager;
                _logger = logger;
            }

            public async Task<ApiResult<string>> Handle(OnLeaveService request, CancellationToken cancellationToken)
            {
                var group = await _restManager.LookupAsync(new ClientsGroup { Id = request.Request.GroupId });

                if (group == null)
                {
                    var errorMessage = CustomExceptionCodes.GroupNotFound.GetEnumDescription(request.Request.GroupId);
                    await _logger.LogToConsoleAsync(errorMessage);

                    request.Error = new Error
                    {
                        ErrorCode = CustomExceptionCodes.GroupNotFound,
                        ErrorMessage = errorMessage,
                        HttpStatus = System.Net.HttpStatusCode.NotFound
                    };

                    return ApiResult<string>.ERROR(request.Error);
                }

                await _restManager.OnLeaveAsync(new ClientsGroup { Id = request.Request.GroupId });
                await _logger.LogToConsoleAsync($"Group {request.Request.GroupId} has left.");

                return ApiResult<string>.OK("Group has successfully left");
            }
        }
    }
}
