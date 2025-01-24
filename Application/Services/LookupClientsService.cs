using Application.Services.Core;
using Domain.Models;
using MediatR;
using Application.Interfaces;
using Domain.Enums;
using Application.DTOs.Requests;
using Application.Extensions;
using Application.Interfaces.Common;
using Application.DTOs.Responses;
using AutoMapper;

namespace Application.Services
{
    public class LookupClientsService : BaseRequest<ApiResult<TableDto>>
    {
        public LookupClientsRequest Request { get; set; }

        public LookupClientsService(LookupClientsRequest request) => Request = request;

        public class LookupClientsServiceHandler : IRequestHandler<LookupClientsService, ApiResult<TableDto>>
        {
            private readonly IRestManager _restManager;
            private readonly ILogger _logger;
            private readonly IMapper _mapper;

            public LookupClientsServiceHandler(IRestManager restManager, ILogger logger, IMapper mapper)
            {
                _restManager = restManager;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<ApiResult<TableDto>> Handle(LookupClientsService request, CancellationToken cancellationToken)
            {
                try
                {
                    var table = await _restManager.LookupAsync(new ClientsGroup { Id = request.Request.GroupId });

                    if (table == null)
                    {
                        var errorMessage = CustomExceptionCodes.GroupNotAssociated.GetEnumDescription(request.Request.GroupId);
                        await _logger.LogToConsoleAsync(errorMessage);

                        request.Error = new Error
                        {
                            ErrorCode = CustomExceptionCodes.GroupNotFound,
                            ErrorMessage = errorMessage,
                            HttpStatus = System.Net.HttpStatusCode.NotFound
                        };

                        return ApiResult<TableDto>.ERROR(request.Error);
                    }

                    await _logger.LogToConsoleAsync($"Group {request.Request.GroupId} is seated at table of size {table.Size}.");
                    var result = _mapper.Map<TableDto>(table);
                    return ApiResult<TableDto>.OK(result);
                }
                catch (Exception ex)
                {
                    await _logger.LogToConsoleAsync($"Error during lookup for group {request.Request.GroupId}: {ex.Message}");

                    request.Error = new Error
                    {
                        ErrorCode = CustomExceptionCodes.UnHandledException,
                        ErrorMessage = ex.Message,
                        HttpStatus = System.Net.HttpStatusCode.InternalServerError
                    };

                    return ApiResult<TableDto>.ERROR(request.Error);
                }
            }
        }
    }
}
