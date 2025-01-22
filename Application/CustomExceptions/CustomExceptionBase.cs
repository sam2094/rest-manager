namespace Application.CustomExceptions
{
    public class CustomExceptionBase : Exception
    {
        public CustomExceptionBase(string exceptionDescription) : base(exceptionDescription) { }
    }
}