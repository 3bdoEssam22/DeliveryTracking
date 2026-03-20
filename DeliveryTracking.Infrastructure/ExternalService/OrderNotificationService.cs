using DeliveryTracking.Core.Contracts;
using DeliveryTracking.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Infrastructure.ExternalService
{
    public class OrderNotificationService(
        IHubContext<OrderTrackingHub> _orderHub,
        IHubContext<NotificationHub> _notificationHub,
        IConnectionMappingService _connectionMapping) : IOrderNotificationService
    {
        public async Task BroadcastStatusUpdateAsync(Guid orderId, string newStatus)
        {
            await _orderHub.Clients.Group(orderId.ToString())
                .SendAsync("ReceiveStatusUpdate", orderId, newStatus, DateTime.UtcNow);
        }

        public async Task PushNotificationAsync(string userId, string message, string type)
        {
            var connectionId = _connectionMapping.GetConnectionId(userId);
            if (connectionId is not null)
                await _notificationHub.Clients.Client(connectionId)
                    .SendAsync("ReceiveNotification", message, type, DateTime.UtcNow);
        }
    }
}
