using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;
using TFG.ServiceContracts.Models;

namespace TFG.Bot.Cards
{
    public class GetAdaptiveCard
    {
        public static Activity Recipe(Activity activity, List<EdamamRecipe> recipes, IMessagesService messagesService)
        {
            var result = new List<Attachment>();

            var reply = activity.CreateReply();

            foreach (var item in recipes)
            {
                var body = new List<AdaptiveContainer>();

                #region Tittle
                var tittle = new AdaptiveTextBlock
                {
                    Text = item.label,
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Medium
                };

                body.Add(new AdaptiveContainer { Items = new List<AdaptiveElement> { tittle } });
                #endregion

                #region Imagen
                var imagen = new AdaptiveImage { Url = new Uri(item.image), Size = AdaptiveImageSize.Large, AltText = item.label };

                body.Add(new AdaptiveContainer { Items = new List<AdaptiveElement> { imagen } });
                #endregion

                #region Allergies
                string allergyText = string.Empty;

                foreach (var allergy in item.healthLabels)
                {
                    allergyText += messagesService.Get(allergy).Value;
                }

                var allergyEmojis = new AdaptiveTextBlock
                {
                    Text = allergyText,
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Medium
                };

                body.Add(new AdaptiveContainer { Items = new List<AdaptiveElement> { allergyEmojis } });
                #endregion

                #region Ingredients
                var ingredientContainer = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

                ingredientContainer.Items.Add(new AdaptiveTextBlock { Text = "Ingredientes:", Weight = AdaptiveTextWeight.Bolder, Size = AdaptiveTextSize.Medium, });

                foreach (var ingredient in item.ingredientLines)
                {
                    ingredientContainer.Items.Add(new AdaptiveTextBlock { Text = ingredient, Weight = AdaptiveTextWeight.Default, Size = AdaptiveTextSize.Default });
                }

                body.Add(ingredientContainer);
                #endregion

                #region Button
                var action = new AdaptiveOpenUrlAction { Title = "ver mas", Url = new Uri(item.url), };

                List<AdaptiveAction> actions = new List<AdaptiveAction> { action };
                #endregion

                result.AddRange(CreateAdaptiveCard(body, actions));
            }

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            reply.Attachments = result;

            reply.Text = messagesService.Get(MessagesKey.Key.RecipeSearch_result.ToString()).Value;

            return reply;
        }

        private static List<Attachment> CreateAdaptiveCard(List<AdaptiveContainer> adaptiveContainers, List<AdaptiveAction> actions = null)
        {
            var attachments = new List<Attachment>();

            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>(),
                Actions = new List<AdaptiveAction>()
            };

            adaptiveContainers.ForEach(x => card.Body.Add(x));

            if (actions != null)
            {
                actions.ForEach(x => card.Actions.Add(x));
            }

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
