using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Helper
{
    public class BotRouter
    {
        public static async Task<DialogTurnResult> Router(WaterfallStepContext stepContext, IStatePropertyAccessor<ConversationData> _conversationStateAccessor, IMessagesService messagesService)
        {
            var message = messagesService.Get(MessagesKey.Key.ResultIntent.ToString());

            var conversationData = await _conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            var lr = conversationData.LuisResult;

            switch (lr.TopScoringIntent.Intent)
            {
                case "":
                    break;
                default:
                    await stepContext.Context.SendActivityAsync(string.Format(message.Value, lr.TopScoringIntent.Intent));
                    break;
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
