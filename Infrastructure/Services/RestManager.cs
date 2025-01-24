using Application.Interfaces;
using Domain.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Infrastructure.Services
{
    public class RestManager : IRestManager
    {
        private readonly List<Table> _tables; // all tables in the restaurant sorted by size
        private readonly Channel<ClientsGroup> _channel; // for managing incoming groups
        private readonly ConcurrentDictionary<Guid, Table> _groupTableMapping = new(); // tracks which group is at which table
        private readonly SemaphoreSlim _semaphore = new(1, 1); // to keep things safe with multiple theads
        private readonly List<ClientsGroup> _queue = new(); // queue for groups waiting for tables
        private readonly SemaphoreSlim _queueEvent = new(0, int.MaxValue); // signals when the queue needs to be checked

        public RestManager(List<Table> tables)
        {
            _tables = tables.OrderBy(t => t.Size).ToList();
            _channel = Channel.CreateUnbounded<ClientsGroup>();
            StartProcessingQueue(); // starts background process to handle the queue
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
                    return;
                }

                if (_groupTableMapping.TryRemove(group.Id, out var table))
                {
                    table.ClientGroupIds.Remove(group.Id);
                    if (table.ClientGroupIds.Count == 0)
                    {
                        table.IsOccupied = false;
                        await _channel.Writer.WriteAsync(null); // send signal to recheck the queue by writing null to the chanel
                        _queueEvent.Release(); // notify the queue processor that an event occurred
                    }
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

        private void StartProcessingQueue()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await _queueEvent.WaitAsync(); // waiting signal to start handling queue

                    while (_channel.Reader.TryRead(out var group))
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            if (group == null) // table is free start checking the queue again
                            {
                                ProcessQueue();
                                continue;
                            }

                            _queue.Add(group);

                            // sorting by size and arrival time
                            _queue.Sort((g1, g2) =>
                            {
                                int sizeComparison = g1.Size.CompareTo(g2.Size);
                                if (sizeComparison != 0)
                                    return sizeComparison;

                                return g1.ArrivalTime.CompareTo(g2.ArrivalTime);
                            });

                            ProcessQueue();
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }
                }
            });
        }

        private void ProcessQueue()
        {
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

        private void SeatGroupAtTable(ClientsGroup group, Table table)
        {
            table.IsOccupied = true;
            table.ClientGroupIds.Add(group.Id);
            _groupTableMapping[group.Id] = table;
        }

        // additional functionalities
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
    }
}
