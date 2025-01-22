using Application.Interfaces;
using Domain.Models;

namespace Infrastructure.Services
{
    public class RestManager : IRestManager
    {
        private readonly List<Table> _tables;
        private readonly Queue<ClientsGroup> _queue;

        public RestManager()
        {
            _tables = new List<Table>
            {
                new Table { Id = 1, Size = 2 },
                new Table { Id = 2, Size = 4 },
                new Table { Id = 3, Size = 6 }
            };
            _queue = new Queue<ClientsGroup>();
        }

        public bool TrySeatGroup(ClientsGroup group)
        {
            var availableTable = _tables.FirstOrDefault(t => !t.IsOccupied && t.Size >= group.Size);
            if (availableTable != null)
            {
                availableTable.IsOccupied = true;
                availableTable.ClientGroupIds.Add(group.Id);
                return true;
            }

            _queue.Enqueue(group);
            return false;
        }

        public void FreeTable(int tableId)
        {
            var table = _tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null)
            {
                table.IsOccupied = false;
                table.ClientGroupIds.Clear();
            }
        }
    }
}