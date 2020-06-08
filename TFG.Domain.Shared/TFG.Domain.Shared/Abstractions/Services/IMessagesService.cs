using TFG.ServiceContracts.Models;

namespace TFG.Domain.Shared.Abstractions.Services
{
    public interface IMessagesService
    {
        public Message Get(string messageId);
    }
}
