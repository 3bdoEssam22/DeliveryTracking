using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Core.Contracts
{
    public interface IOrderNotificationService
    {
        Task BroadcastStatusUpdateAsync(Guid orderId, string newStatus);
        Task PushNotificationAsync(string userId, string message, string type);
    }
}
