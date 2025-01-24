using Domain.Models;

namespace Application.Interfaces
{
    public interface IRestManager
    {
        Task OnArriveAsync(ClientsGroup group);
        Task OnLeaveAsync(ClientsGroup group);
        Task<Table> LookupAsync(ClientsGroup group);

        // additional functionalities
        Task<IEnumerable<Table>> GetAvailableTablesAsync();
        Task<IEnumerable<Table>> GetOccupiedTablesAsync();
    }
}
