using eDereva.Core.Contracts.Requests;

namespace eDereva.Core.Services
{
    public interface ISmsService
    {
        Task<bool> SendMessageAsync(Sms sms);
        Task<bool> SendBulkMessagesAsync(IEnumerable<Sms> messages);
        Task<string> GetDeliveryStatusAsync(string messageId);
    }
}
