using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using TFG;

namespace Microsoft.BotBuilderSamples
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Dialog Dialog;
        protected readonly BotState UserState;

        protected readonly ILogger Logger;
        protected readonly IBotServices BotServices;

        public DialogBot(ConversationState ConversationState, UserState UserState, T Dialog, ILogger<DialogBot<T>> Logger, IBotServices BotServices)
        {
            this.ConversationState = ConversationState;
            this.UserState = UserState;
            this.Dialog = Dialog;
            this.Logger = Logger;
            this.BotServices = BotServices;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await RecognizerResultAsync(turnContext, cancellationToken);

            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome to DiaBot "), cancellationToken);
                }
            }
        }

        private async Task RecognizerResultAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var ConversationStateAccessor = ConversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationState = await ConversationStateAccessor.GetAsync(turnContext, () => new ConversationData());

            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            conversationState.RecognizerResult = await BotServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);

            conversationState.LuisResult = conversationState.RecognizerResult.Properties["luisResult"] as LuisResult;
        }
    }
}
