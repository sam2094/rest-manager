using Application.Extensions;
using Domain.Enums;
using System.Text;
using System.Text.Json;

namespace Application.CustomExceptions
{
    public static class ExceptionMessageBuilder
    {
        public static string Build(
            CustomExceptionCodes customException,
            Exception originalException = null,
            string externalServiceResponse = null,
            object model = null)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"* Custom Exception: \n: {customException.GetEnumDescription()}");

            if (originalException != null)
            {
                builder.AppendLine($"\n * Original exception: \n {originalException.Message}");
            }

            if (externalServiceResponse != null)
            {
                builder.AppendLine($"\n External Service Response: \n {externalServiceResponse}");
            }

            if (model != null)
            {
                builder.AppendLine($"\n Model: \n {JsonSerializer.Serialize(model)}");
            }

            return builder.ToString();
        }
    }
}