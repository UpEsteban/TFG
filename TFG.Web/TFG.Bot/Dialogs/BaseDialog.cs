using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Threading;
using System.Threading.Tasks;
using TFG.Bot.Resources;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Dialogs
{
    public class BaseDialog : ComponentDialog
    {
        private IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IMessagesService messagesService;

        public BaseDialog(string id, ConversationState conversationState, IMessagesService messagesService)
            : base(id)
        {
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            this.messagesService = messagesService;
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
                return result;
            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var cancelMessage = messagesService.Get(MessagesKey.Key.Cancel.ToString());

            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var conversation = await conversationStateAccessor.GetAsync(innerDc.Context, () => new ConversationData());

                var luisResult = conversation.LuisResult;

                if (luisResult.TopScoringIntent.Score < 0.9)
                    return null;

                switch (luisResult.TopScoringIntent.Intent)
                {
                    case Luis.Welcome_Intent:
                        // Cancell all dialogs.
                        await innerDc.Context.SendActivityAsync(cancelMessage.Value, cancellationToken: cancellationToken);
                        return await innerDc.CancelAllDialogsAsync();
                }
            }
            return null;
        }
    }
}
