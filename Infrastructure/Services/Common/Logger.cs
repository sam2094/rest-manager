using Serilog;

namespace Infrastructure.Services.Common
{
    public class Logger : Application.Interfaces.Common.ILogger
    {
        public async Task LogToConsoleAsync(string message)
        {
            Log.Information(message);
            await Task.CompletedTask;
        }
    }
}