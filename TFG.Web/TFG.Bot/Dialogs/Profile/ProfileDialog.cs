using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards.Rendering;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using TFG.Bot.Cards;
using TFG.Bot.Helper;
using TFG.Bot.Resources;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Dialogs.Profile
{
    public class ProfileDialog : BaseDialog
    {
        private readonly IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IMessagesService messagesService;

        public ProfileDialog(ConversationState conversationState, IMessagesService messagesService)
            : base(nameof(ProfileDialog), conversationState, messagesService)
        {
            // Get Conversation State Accessor
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            this.messagesService = messagesService;

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                ProfileStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ProfileStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversation = await conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            var message = messagesService.Get(MessagesKey.Key.Profile_initial.ToString()).Value;

            await stepContext.Context.SendActivityAsync(message);

            var messageAllergies = AllergyMessage(conversation.User.Allergies);

            await stepContext.Context.SendActivityAsync(messageAllergies);

            var reply = MessageFactory.Text(messagesService.Get(MessagesKey.Key.Profile_lastSearch.ToString()).Value);

            reply.SuggestedActions = LastSearchMessage(new List<string> { "Huevos rotos con patatas", "bacon con patatas", "sancocho colombiano", "fabada asturiana" });

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.EndDialogAsync();
        }

        private string AllergyMessage(List<string> allergies)
        {
            var message = messagesService.Get(MessagesKey.Key.Profile_allergies.ToString()).Value;

            var allergiesMessage = string.Empty;

            foreach (var item in allergies)
            {
                var emoji = messagesService.Get(item + "_Emoji").Value;

                allergiesMessage += " - " + item + ":" + emoji;
            }

            return string.Format(message, allergiesMessage);
        }

        private SuggestedActions LastSearchMessage(List<string> lastSearchs)
        {
            var actions = new List<CardAction>();

            foreach (var item in lastSearchs.Take(5))
            {
                actions.Add(new CardAction { Title = item, Type = ActionTypes.ImBack, Value = item });
            }

            return new SuggestedActions { Actions = actions };
        }
    }
}
