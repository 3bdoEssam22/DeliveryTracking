using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryTracking.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
