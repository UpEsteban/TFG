using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TFG.Helper;

namespace TFG.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private IStatePropertyAccessor<ConversationData> _conversationStateAccessor;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(ConversationState conversationState, UserState userState)
            : base(nameof(MainDialog))
        {
            // Get Conversation State Accessor
            _conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Initial Steps
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await BotRouter.Router(stepContext, _conversationStateAccessor);
        }
    }
}
