using Application.Services.Core;
using Domain.Models;
using MediatR;
using Application.Interfaces;
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
                var group = new ClientsGroup
                {
                    Id = Guid.NewGuid(),
                    Size = request.Request.GroupSize
                };

                await _restManager.OnArriveAsync(group);
                await _logger.LogToConsoleAsync($"Group {group.Id} successfully arrived.");

                return ApiResult<string>.OK(group.Id.ToString());
            }
        }
    }
}
