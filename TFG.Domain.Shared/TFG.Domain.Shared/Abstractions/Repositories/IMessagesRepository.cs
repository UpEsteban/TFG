using TFG.Domain.Shared.Models;

namespace TFG.Domain.Shared.Abstractions.Repositories
{
    public interface IMessagesRepository
    {
        public MessageDTO Get(string messageId, string language = "es");
    }
}
