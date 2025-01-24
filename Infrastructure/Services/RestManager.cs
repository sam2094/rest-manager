using Application.Interfaces;
using Domain.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Infrastructure.Services
{
    public class RestManager : IRestManager
    {
        private readonly List<Table> _tables; 
        private readonly Channel<ClientsGroup> _channel; 
        private readonly ConcurrentDictionary<Guid, Table> _groupTableMapping = new(); 
        private readonly SemaphoreSlim _semaphore = new(1, 1); 
        private readonly List<ClientsGroup> _queue = new();

        private readonly SemaphoreSlim _queueEvent = new(0, int.MaxValue); 

        public RestManager(List<Table> tables)
        {
            _tables = tables.OrderBy(t => t.Size).ToList(); 
            _channel = Channel.CreateUnbounded<ClientsGroup>(); 
            StartProcessingQueue();
        }

        public async Task OnArriveAsync(ClientsGroup group)
        {
            await _semaphore.WaitAsync();
            try
            {
                await _channel.Writer.WriteAsync(group);
                _queueEvent.Release();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task OnLeaveAsync(ClientsGroup group)
        {
            await _semaphore.WaitAsync();
            try
            {
                var existingGroup = _queue.FirstOrDefault(g => g.Id == group.Id);
                if (existingGroup != null)
                {
                    _queue.Remove(existingGroup);
                }

                if (_groupTableMapping.TryRemove(group.Id, out var table))
                {
                    table.ClientGroupIds.Remove(group.Id);
                    if (table.ClientGroupIds.Count == 0)
                    {
                        table.IsOccupied = false;
                    }

                    _queueEvent.Release();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Table> LookupAsync(ClientsGroup group)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _groupTableMapping.TryGetValue(group.Id, out var table) ? table : null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Table>> GetAvailableTablesAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tables.Where(t => !t.IsOccupied).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Table>> GetOccupiedTablesAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tables.Where(t => t.IsOccupied).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void StartProcessingQueue()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await _queueEvent.WaitAsync();

                    while (_channel.Reader.TryRead(out var group))
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            _queue.Add(group);

                            _queue.Sort((g1, g2) =>
                            {
                                int sizeComparison = g1.Size.CompareTo(g2.Size); 
                                return sizeComparison != 0 ? sizeComparison : g1.Id.CompareTo(g2.Id);
                            });

                            var nextGroup = _queue.FirstOrDefault();
                            if (nextGroup != null)
                            {
                                var table = _tables.FirstOrDefault(t => t.Size >= nextGroup.Size && !t.IsOccupied);
                                if (table != null)
                                {
                                    SeatGroupAtTable(nextGroup, table);
                                    _queue.Remove(nextGroup);
                                }
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }
                }
            });
        }

        private void SeatGroupAtTable(ClientsGroup group, Table table)
        {
            table.IsOccupied = true;
            table.ClientGroupIds.Add(group.Id);
            _groupTableMapping[group.Id] = table;
        }
    }
}
