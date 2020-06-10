using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Dialogs.SearchRecipe
{
    public class SearchRecipeDialog : BaseDialog
    {
        private readonly IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IMessagesService messagesService;

        public SearchRecipeDialog(ConversationState conversationState, IMessagesService messagesService)
            : base(nameof(SearchRecipeDialog), conversationState, messagesService)
        {
            // Get Conversation State Accessor
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            this.messagesService = messagesService;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }
    }
}
