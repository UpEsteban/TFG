using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace TFG.Helper
{
    public class BotRouter
    {
        public static async Task<DialogTurnResult> Router(WaterfallStepContext stepContext, IStatePropertyAccessor<ConversationData> _conversationStateAccessor)
        {
            var conversationData = await _conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            var lr = conversationData.LuisResult;

            switch (lr.TopScoringIntent.Intent)
            {
                case "":
                    break;
                default:
                    await stepContext.Context.SendActivityAsync($"INTENT: {lr.TopScoringIntent.Intent}");
                    break;
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
