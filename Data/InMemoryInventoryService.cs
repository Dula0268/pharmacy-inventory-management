using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PharmacyInventory.Models;
using PharmacyInventory.Services;

namespace PharmacyInventory.Data
{
    public class InMemoryInventoryService : IInventoryService
    {
        private readonly ConcurrentDictionary<Guid, InventoryItem> _items = new();

        public Task AddAsync(InventoryItem item)
        {
            _items[item.Id] = item;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _items.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return Task.FromResult(_items.Values.AsEnumerable());
        }

        public Task<InventoryItem?> GetByIdAsync(Guid id)
        {
            _items.TryGetValue(id, out var item);
            return Task.FromResult(item);
        }

        public Task UpdateAsync(InventoryItem item)
        {
            _items[item.Id] = item;
            return Task.CompletedTask;
        }
    }
}
