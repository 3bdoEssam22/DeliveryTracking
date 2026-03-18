using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Core.Entities.OrderModule
{
    public enum OrderStatus
    {
        Pending = 0,
        Assigned = 1,
        PickedUp = 2,
        OnTheWay = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
