using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Core.Contracts
{
    public interface IDataInitializer
    {
        Task InitializeAdminAndRolesAsync();
    }
}
