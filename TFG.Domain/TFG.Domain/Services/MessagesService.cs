using TFG.Domain.Shared.Abstractions.Domains;
using TFG.Domain.Shared.Abstractions.Services;
using TFG.ServiceContracts.Models;

namespace TFG.Domain.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IMessagesDomain domain;

        public MessagesService(IMessagesDomain domain)
        {
            this.domain = domain;
        }

        public Message Get(string messageId)
        {
            return domain.Get(messageId);
        }
    }
}
