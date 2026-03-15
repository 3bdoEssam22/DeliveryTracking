using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages
{
    public class Email
    {
        public string Subject { get; set; } = null!;
        public string To { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
