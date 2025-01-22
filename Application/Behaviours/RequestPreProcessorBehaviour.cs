using Application.Interfaces.Common;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace Application.Behaviours
{
    public class RequestPreProcessorBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly HttpContext _httpContext;

        public RequestPreProcessorBehaviour(
            ILogger logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"Mediatr: {typeof(TRequest)}\n");

            var model = request.GetType().GetProperty("Model" + "\n");

            if (model is not null)
                stringBuilder.Append("Params: " + JsonConvert.SerializeObject(model));

            await _logger.LogToConsoleAsync($"Mediatr: {stringBuilder.ToString()}\n");
        }
    }
}