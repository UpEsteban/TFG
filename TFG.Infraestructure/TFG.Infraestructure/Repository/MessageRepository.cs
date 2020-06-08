using System.Collections.Generic;
using System.Linq;
using System.Text;
using TFG.Domain.Shared.Abstractions.Repositories;
using TFG.Domain.Shared.Models;
using TFG.Infraestructure.Helpers;

namespace TFG.Infraestructure.Repository
{
    public class MessagesRepository : IMessagesRepository
    {

#if DEBUG
        private const string MessagesJson = "Resources/Messages/Messages.json";
#else
        private const string MessagesJson = "Resources\\Messages\\Messages.json";
#endif

        public MessageDTO Get(string messageId, string language = "es")
        {
            JsonDataReader jsonDataReader = new JsonDataReader();

            var messages = jsonDataReader.ReadDataFromJson<List<MessageDTO>>(MessagesJson, Encoding.UTF8);

            var result = messages.Where(x => x.Language.Equals(language) && x.MessageId.Equals(messageId)).ToList();

            return result.FirstOrDefault();
        }
    }
}
