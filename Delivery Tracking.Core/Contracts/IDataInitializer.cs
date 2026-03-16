using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery_Tracking.Core.Contracts
{
    public interface IDataInitializer
    {
        Task InitializeAdminAndRolesAsync();
    }
}
