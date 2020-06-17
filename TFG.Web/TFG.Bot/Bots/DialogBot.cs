using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TFG;
using TFG.Bot.Cards;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace Microsoft.BotBuilderSamples
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog dialog;
        protected readonly BotState userState;
        protected readonly BotState conversationState;

        protected readonly ILogger logger;
        protected readonly IBotServices botServices;
        protected readonly IMessagesService messagesService;

        public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IBotServices botServices, IMessagesService messagesService)
        {
            this.conversationState = conversationState;
            this.userState = userState;
            this.dialog = dialog;
            this.logger = logger;
            this.botServices = botServices;
            this.messagesService = messagesService;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await RecognizerResultAsync(turnContext, cancellationToken);

            SetUserData(turnContext);

            // Set spanish language
            turnContext.Activity.Locale = "es-ES";

            // Run the Dialog with the new message Activity.
            await dialog.RunAsync(turnContext, conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var message = messagesService.Get(MessagesKey.Key.Welcome.ToString());

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(message.Value), CancellationToken.None);
                    await turnContext.SendActivityAsync(HeroCards.Welcome((Activity)turnContext.Activity, messagesService));
                }
            }
        }

        private async Task RecognizerResultAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var ConversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversation = await ConversationStateAccessor.GetAsync(turnContext, () => new ConversationData());

            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            conversation.RecognizerResult = await botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);

            conversation.LuisResult = conversation.RecognizerResult.Properties["luisResult"] as LuisResult;
        }

        private void SetUserData(ITurnContext<IMessageActivity> turnContext)
        {
            var conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            var conversation = conversationStateAccessor.GetAsync(turnContext, () => new ConversationData()).Result;

            if (conversation.User != null)
            {
                turnContext.Activity.From.Properties = JObject.FromObject(conversation.User);
            }
        }
    }
}
