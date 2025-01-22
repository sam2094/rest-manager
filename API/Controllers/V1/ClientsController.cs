using Application.DTOs.Requests;
using Application.Extension;
using Application.Services;
using Application.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;

[Route("api/v{version:apiVersion}/clients")]
[ApiController]
[ApiVersion("1.0")]
public class ClientsController : BaseController
{
    [HttpPost("add")]
    public async Task<ActionResult<ApiResult<string>>> AddClients([FromBody] AddClientsRequest input)
    {
        var service = new AddClientsService(input);
        return await Mediator.HandleRequest(service, HttpContext);
    }
}
