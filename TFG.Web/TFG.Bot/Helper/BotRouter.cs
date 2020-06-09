using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading.Tasks;
using TFG.Bot.Helper;
using TFG.Bot.Resources;
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

            LuisResult lr = conversationData.LuisResult;

            var subdialog = GetSubdialog(lr);

            if (!string.IsNullOrEmpty(subdialog))
            {
                return await stepContext.BeginDialogAsync(subdialog);
            }

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

        private static string GetSubdialog(LuisResult luisResult)
        {
            var entity = luisResult.Entities.Where(x => x.Type.Equals(Luis.SubDialogs)).FirstOrDefault();

            if (entity != null)
            {
                var entityNormalizeName = LuisHelper.GetNormalizedValueFromEntity(entity);

                return entityNormalizeName + "Dialog";
            }

            return string.Empty;
        }
    }
}
