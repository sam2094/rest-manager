using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Extension;
using Application.Services;
using Application.Services.Core;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class RestManagerController : BaseController
    {
        [HttpPost("arrive")]
        public async Task<ActionResult<ApiResult<string>>> OnArrive([FromBody] AddClientsRequest input)
        {
            var service = new OnArriveService(input);
            return await Mediator.HandleRequest(service, HttpContext);
        }

        [HttpPost("leave")]
        public async Task<ActionResult<ApiResult<string>>> OnLeave([FromBody] RemoveClientsRequest input)
        {
            var service = new OnLeaveService(input);
            return await Mediator.HandleRequest(service, HttpContext);
        }

        [HttpGet("lookup")]
        public async Task<ActionResult<ApiResult<TableDto>>> Lookup([FromQuery] LookupClientsRequest input)
        {
            var service = new LookupClientsService(input);
            return await Mediator.HandleRequest(service, HttpContext);
        }
    }
}