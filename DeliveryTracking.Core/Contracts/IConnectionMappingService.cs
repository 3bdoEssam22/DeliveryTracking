using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Core.Contracts
{
    public interface IConnectionMappingService
    {
        void Add(string userId, string connectionId);
        void Remove(string userId);
        string? GetConnectionId(string userId);
    }
}
