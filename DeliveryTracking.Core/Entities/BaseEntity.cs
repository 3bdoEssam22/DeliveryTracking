using System;
using System.Collections.Generic;
using System.Text;

namespace Delivery_Tracking.Core.Entities
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
