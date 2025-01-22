using System.ComponentModel;

namespace Domain.Enums
{
    public enum CustomExceptionCodes
    {
        [Description("An unexpected error occurred while processing the request")]
        UnHandledException = 1_0_0_0,

        [Description("Validation exception occurred")]
        ValidationException = 1_0_0_1
    }
}