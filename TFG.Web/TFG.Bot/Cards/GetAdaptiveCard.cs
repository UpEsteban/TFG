using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Cards
{
    public class GetAdaptiveCard
    {
        private static List<Attachment> CreateAdaptiveCard(List<AdaptiveContainer> adaptiveContainers)
        {
            var attachments = new List<Attachment>();

            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>()
            };

            adaptiveContainers.ForEach(x => card.Body.Add(x));

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(card.ToJson()),
            };

            attachments.Add(adaptiveCardAttachment);

            return attachments;
        }

        private static AdaptiveFact CreateFact(string title, string value) => new AdaptiveFact { Title = title, Value = value };
    }
}
