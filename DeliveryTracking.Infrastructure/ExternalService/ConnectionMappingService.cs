using DeliveryTracking.Core.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Infrastructure.ExternalService
{
    public class ConnectionMappingService : IConnectionMappingService
    {
        private readonly ConcurrentDictionary<string, string> _connections = new();

        public void Add(string userId, string connectionId)
            => _connections[userId] = connectionId;

        public void Remove(string userId)
            => _connections.TryRemove(userId, out _);

        public string? GetConnectionId(string userId)
            => _connections.TryGetValue(userId, out var connectionId) ? connectionId : null;
    }

}

