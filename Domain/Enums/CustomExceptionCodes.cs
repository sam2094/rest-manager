using System.ComponentModel;

namespace Domain.Enums
{
    public enum CustomExceptionCodes
    {
        [Description("An unexpected error occurred while processing the request")]
        UnHandledException = 1_0_0_0,

        [Description("Validation exception occurred")]
        ValidationException = 1_0_0_1,

        [Description("The client group with the specified ID '{0}' could not be found in the system")]
        GroupNotFound = 2_0_0_1,

        [Description("The client group with the specified ID '{0}' is not associated with any table. It may still be in the queue or may have left")]
        GroupNotAssociated = 2_0_0_2,
    }
}