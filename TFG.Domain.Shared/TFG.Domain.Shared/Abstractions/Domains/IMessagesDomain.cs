using TFG.ServiceContracts.Models;

namespace TFG.Domain.Shared.Abstractions.Domains
{
    public interface IMessagesDomain
    {
        public Message Get(string messageId);
    }
}
