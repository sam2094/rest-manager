using Application.Services.Core;
using Domain.Models;
using MediatR;
using Application.Interfaces;
using Domain.Enums;
using Application.DTOs.Requests;
using Application.Interfaces.Common;
using Application.Extensions;
using System.Text.RegularExpressions;

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
                try
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
                catch (Exception ex)
                {
                    await _logger.LogToConsoleAsync($"Error processing leave request for group {request.Request.GroupId}: {ex.Message}");

                    request.Error = new Error
                    {
                        ErrorCode = CustomExceptionCodes.UnHandledException,
                        ErrorMessage = ex.Message,
                        HttpStatus = System.Net.HttpStatusCode.InternalServerError
                    };

                    return ApiResult<string>.ERROR(request.Error);
                }
            }
        }
    }
}
