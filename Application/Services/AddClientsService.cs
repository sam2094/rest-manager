using Application.Services.Core;
using Domain.Models;
using MediatR;
using Application.Interfaces;
using Domain.Enums;
using Application.DTOs.Requests;
using Application.Interfaces.Common;

namespace Application.Services
{
    public class OnArriveService : BaseRequest<ApiResult<string>>
    {
        public AddClientsRequest Request { get; set; }

        public OnArriveService(AddClientsRequest request) => Request = request;

        public class OnArriveServiceHandler : IRequestHandler<OnArriveService, ApiResult<string>>
        {
            private readonly IRestManager _restManager;
            private readonly ILogger _logger;

            public OnArriveServiceHandler(IRestManager restManager, ILogger logger)
            {
                _restManager = restManager;
                _logger = logger;
            }

            public async Task<ApiResult<string>> Handle(OnArriveService request, CancellationToken cancellationToken)
            {
                try
                {
                    var group = new ClientsGroup
                    {
                        Id = Guid.NewGuid(),
                        Size = request.Request.GroupSize
                    };

                    await _restManager.OnArriveAsync(group);

                    await _logger.LogToConsoleAsync($"Group {group.Id} successfully arrived.");
                    return ApiResult<string>.OK(group.Id.ToString());
                }
                catch (Exception ex)
                {
                    await _logger.LogToConsoleAsync($"Error processing group: {ex.Message}");

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
