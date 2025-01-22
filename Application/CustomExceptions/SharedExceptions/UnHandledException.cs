namespace Application.CustomExceptions.SharedExceptions
{
    public class UnHandledException : CustomExceptionBase
    {
        public UnHandledException(string exceptionDescription) : base(exceptionDescription) { }
    }
}