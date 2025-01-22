using MediatR.Pipeline;
using Newtonsoft.Json;
using System.Text;
using Application.Services.Core;
using Application.Interfaces.Common;

namespace Application.Behaviours
{
    public class RequestPostProcessorBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : BaseRequest<TResponse>
    {
        private readonly ILogger _logger;

        public RequestPostProcessorBehaviour(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Response date:" + DateTime.Now + "\n");
            builder.Append("Response:" + JsonConvert.SerializeObject(response) + "\n");
            builder.Append("--------Request Ended--------" + "\n");
            await _logger.LogToConsoleAsync(builder.ToString());
        }
    }
}