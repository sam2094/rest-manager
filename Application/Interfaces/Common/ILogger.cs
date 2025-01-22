namespace Application.Interfaces.Common
{
    public interface ILogger
    {
        Task LogToConsoleAsync(string message);
    }
}
