using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using TFG.Bot.Cards;
using TFG.Bot.Resources;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;
using TFG.ServiceContracts.Models;

namespace TFG.Bot.Dialogs.SearchRecipe
{
    public class SearchRecipeDialog : BaseDialog
    {
        private readonly string recipePrompt = "recipePrompt";

        private readonly IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IStatePropertyAccessor<SearchRecipeState> recipeStateStateAccessor;

        private readonly IMessagesService messagesService;

        private readonly IEdamamService edamamService;

        public SearchRecipeDialog(ConversationState conversationState, IMessagesService messagesService, IEdamamService edamamService)
            : base(nameof(SearchRecipeDialog), conversationState, messagesService)
        {
            // Get Conversation State Accessor
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            recipeStateStateAccessor = conversationState.CreateProperty<SearchRecipeState>(nameof(SearchRecipeState));

            this.messagesService = messagesService;

            this.edamamService = edamamService;

            AddDialog(new TextPrompt(recipePrompt, RecipeNameValidator));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                NameStepAsync,
                SearchStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = messagesService.Get(MessagesKey.Key.RecipeSearch_askname.ToString()).Value;

            var reply = MessageFactory.Text(message);

            var promptOptions = new PromptOptions
            {
                Prompt = reply,
                Style = ListStyle.Auto
            };

            return await stepContext.PromptAsync(recipePrompt, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> SearchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recipeState = await recipeStateStateAccessor.GetAsync(stepContext.Context, () => new SearchRecipeState());

            var recipe = edamamService.GetRecipeByName(recipeState.recipeName);

            var reply = GetAdaptiveCard.Recipe(stepContext.Context.Activity, recipe, messagesService);

            await stepContext.Context.SendActivityAsync(reply);

            return await stepContext.EndDialogAsync();
        }

        #region Validator
        private async Task<bool> RecipeNameValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                return false;
            }

            var conversation = await conversationStateAccessor.GetAsync(promptContext.Context, () => new ConversationData());

            var recipeState = await recipeStateStateAccessor.GetAsync(promptContext.Context, () => new SearchRecipeState());

            var entity = conversation.LuisResult.Entities.Where(x => x.Type.Equals(Luis.RecipeNameEntity)).FirstOrDefault();

            if (entity != null)
            {
                recipeState.recipeName = entity.Entity;
            }
            else
            {
                recipeState.recipeName = promptContext.Recognized.Value;
            }

            return true;
        }
        #endregion
    }
}
