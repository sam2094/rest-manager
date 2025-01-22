using Domain.Models;

namespace Application.Interfaces
{
    public interface IRestManager
    {
        bool TrySeatGroup(ClientsGroup group);
        void FreeTable(int tableId);
    }
}
