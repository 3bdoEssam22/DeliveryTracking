using Shared.Messages;

namespace DeliveryTracking.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
