using Shared.Messages;

namespace DeliveryTracking.Core.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
