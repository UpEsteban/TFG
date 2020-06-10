using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using TFG.Bot.Helper;
using TFG.Bot.Resources;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Dialogs.DeleteAllergy
{
    public class DeleteAllergyDialog : BaseDialog
    {
        private const string DeleteAllergyPrompt = "DeleteAllergyPrompt";

        private readonly IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IMessagesService messagesService;

        public DeleteAllergyDialog(ConversationState conversationState, IMessagesService messagesService)
            : base(nameof(DeleteAllergyDialog), conversationState, messagesService)
        {
            // Get Conversation State Accessor
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            this.messagesService = messagesService;

            AddDialog(new ChoicePrompt(DeleteAllergyPrompt, AllergyValidator));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                SelectAllergiesStepAsync,
                MoreAllergiesStepAsync,
                MoreAllergiesConfirmationStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversation = await conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            var allergiesEntities = conversation.LuisResult.Entities.Where(x => x.Type.Equals(Luis.AllergyEntity)).ToList();

            if (conversation.User != null)
            {
                if (allergiesEntities.Count() > 0)
                {
                    var allergies = new List<string>();

                    foreach (var item in allergiesEntities)
                    {
                        allergies.Add(LuisHelper.GetNormalizedValueFromEntity(item));
                    }

                    return await stepContext.NextAsync(allergies);
                }

                return await stepContext.NextAsync();
            }

            var message = messagesService.Get(MessagesKey.Key.RemoveAllergy_Error.ToString()).Value;

            await stepContext.Context.SendActivityAsync(message);

            return await stepContext.EndDialogAsync();
        }

        private async Task<DialogTurnResult> SelectAllergiesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversation = await conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            var message = messagesService.Get(MessagesKey.Key.RemoveAllergy_Select.ToString()).Value;

            if (conversation.User.Allergies.Count() <= 0)
            {
                var messageNotFound = messagesService.Get(MessagesKey.Key.RemoveAllergy_Error.ToString()).Value;

                await stepContext.Context.SendActivityAsync(messageNotFound);

                return await stepContext.EndDialogAsync();
            }

            if (stepContext.Result != null && !stepContext.Result.ToString().Equals("Sí"))
            {
                var allergies = (List<string>)stepContext.Result;

                allergies.ForEach(x => conversation.User.Allergies.Remove(x));

                var removed = messagesService.Get(MessagesKey.Key.RemoveAllergy_Removed.ToString()).Value;

                await stepContext.Context.SendActivityAsync(string.Format(removed, string.Join(", ", allergies)));

                return await stepContext.NextAsync();
            }

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(message),
                Choices = ChoiceFactory.ToChoices(conversation.User.Allergies),
                Style = ListStyle.SuggestedAction
            };

            return await stepContext.PromptAsync(DeleteAllergyPrompt, options, cancellationToken);
        }

        private async Task<DialogTurnResult> MoreAllergiesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = messagesService.Get(MessagesKey.Key.RemoveAllergy_More.ToString()).Value;

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(message),
                Style = ListStyle.Auto
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> MoreAllergiesConfirmationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext?.Result)
            {
                return await BackStepAsync(stepContext, 3);
            }

            var conversation = await conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            var message = messagesService.Get(MessagesKey.Key.RemoveAllergy_End.ToString()).Value;

            await stepContext.Context.SendActivityAsync(string.Format(message, string.Join(',', conversation.User.Allergies)));

            return await stepContext.EndDialogAsync();
        }

        #region Validator
        private async Task<bool> AllergyValidator(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            var conversation = await conversationStateAccessor.GetAsync(promptContext.Context, () => new ConversationData());

            var allergy = conversation.LuisResult.Entities.Where(x => x.Type.Equals(Luis.AllergyEntity)).FirstOrDefault();

            if (allergy != null)
            {
                var allergyName = LuisHelper.GetNormalizedValueFromEntity(allergy);

                conversation.User.Allergies.Remove(allergyName);

                var message = messagesService.Get(MessagesKey.Key.RemoveAllergy_Removed.ToString()).Value;

                await promptContext.Context.SendActivityAsync(string.Format(message, allergyName));

                return true;
            }

            return false;
        }
        #endregion

        #region Helper
        private async Task<DialogTurnResult> BackStepAsync(WaterfallStepContext stepContext, int steps)
        {
            // Change step
            stepContext.Stack.Where(x => x.Id.Equals("WaterfallDialog")).FirstOrDefault().State["stepIndex"] =
                (int)stepContext.Stack.Where(x => x.Id.Equals("WaterfallDialog")).FirstOrDefault().State["stepIndex"] - steps;

            //Globals.DisableContextState = true;
            return await stepContext.ContinueDialogAsync();
        }
        #endregion
    }
}
