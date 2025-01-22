namespace Application.CustomExceptions.SharedExceptions
{
    public class CustomException : CustomExceptionBase
    {
        public CustomException(string exceptionDescription) : base(exceptionDescription) { }
    }
}