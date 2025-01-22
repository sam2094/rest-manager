using Application.DTOs.Requests;
using Application.Interfaces;
using Application.Services.Core;
using Domain.Enums;
using Domain.Models;
using MediatR;
using System.Net;

namespace Application.Services
{
    public class AddClientsService : BaseRequest<ApiResult<string>>
    {
        public AddClientsRequest Model { get; set; }

        public AddClientsService(AddClientsRequest model)
        {
            Model = model;
        }

        public class AddClientsServiceHandler : IRequestHandler<AddClientsService, ApiResult<string>>
        {
            private readonly IRestManager _restManager;

            public AddClientsServiceHandler(IRestManager restManager)
            {
                _restManager = restManager;
            }

            public async Task<ApiResult<string>> Handle(AddClientsService request, CancellationToken cancellationToken)
            {
                if (request.Model.GroupSize <= 0)
                {
                    request.Error = new Error
                    {
                        ErrorCode = CustomExceptionCodes.ValidationException,
                        ErrorMessage = "Group size must be greater than zero.",
                        HttpStatus = HttpStatusCode.BadRequest
                    };

                    return ApiResult<string>.ERROR(request.Error);
                }

                var group = new ClientsGroup
                {
                    Id = request.Model.GroupId,
                    Size = request.Model.GroupSize
                };

                var success = _restManager.TrySeatGroup(group);

                if (success)
                {
                    return ApiResult<string>.OK("Group successfully seated.");
                }

                return ApiResult<string>.OK("No available tables. Group added to waiting queue.");
            }
        }
    }
}
