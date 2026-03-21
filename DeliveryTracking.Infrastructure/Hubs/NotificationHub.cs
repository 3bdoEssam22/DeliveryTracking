using DeliveryTracking.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DeliveryTracking.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub(IConnectionMappingService _connectionMapping) : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
                _connectionMapping.Add(userId, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
                _connectionMapping.Remove(userId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}