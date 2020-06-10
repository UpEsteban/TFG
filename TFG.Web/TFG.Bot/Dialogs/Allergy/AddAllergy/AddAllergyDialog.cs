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

namespace TFG.Bot.Dialogs.AddAllergy
{
    public class AddAllergyDialog : BaseDialog
    {
        private const string AllergyPrompt = "AllergyPrompt";

        private readonly IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IMessagesService messagesService;

        public AddAllergyDialog(ConversationState conversationState, IMessagesService messagesService)
            : base(nameof(AddAllergyDialog), conversationState, messagesService)
        {
            // Get Conversation State Accessor
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            this.messagesService = messagesService;

            AddDialog(new ChoicePrompt(AllergyPrompt, AllergyValidator));
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
                conversation.User.Allergies = conversation.User.Allergies ?? new List<string>();
            }
            else
            {
                conversation.User = new ServiceContracts.Models.User { Allergies = new List<string>() };
            }

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

        private async Task<DialogTurnResult> SelectAllergiesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversation = await conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            if (stepContext.Result != null && !stepContext.Result.ToString().Equals("Sí"))
            {
                var allergies = (List<string>)stepContext.Result;

                var added = messagesService.Get(MessagesKey.Key.AddAllergy_Added.ToString()).Value;

                await stepContext.Context.SendActivityAsync(string.Format(added, string.Join(", ", allergies)));

                conversation.User.Allergies.AddRange(allergies);

                conversation.User.Allergies = conversation.User.Allergies.Distinct().ToList();

                return await stepContext.NextAsync();
            }

            var allAllergies = Luis.ValidAllergies;

            var message = messagesService.Get(MessagesKey.Key.AddAllergy_Inital.ToString()).Value;

            if (conversation.User.Allergies.Count() > 0)
            {
                conversation.User.Allergies.ForEach(x => allAllergies.Remove(x));
            }

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(message),
                Choices = ChoiceFactory.ToChoices(allAllergies),
                Style = ListStyle.SuggestedAction
            };

            return await stepContext.PromptAsync(AllergyPrompt, options, cancellationToken);
        }

        private async Task<DialogTurnResult> MoreAllergiesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = messagesService.Get(MessagesKey.Key.AddAllergy_More.ToString()).Value;

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

            var message = messagesService.Get(MessagesKey.Key.AddAllergy_End.ToString()).Value;

            await stepContext.Context.SendActivityAsync(string.Format(message, string.Join(',', conversation.User.Allergies)));

            return await stepContext.EndDialogAsync();
        }

        #region Validators
        private async Task<bool> AllergyValidator(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            var conversation = await conversationStateAccessor.GetAsync(promptContext.Context, () => new ConversationData());

            var allergy = conversation.LuisResult.Entities.Where(x => x.Type.Equals(Luis.AllergyEntity)).FirstOrDefault();

            if (allergy != null)
            {
                var allergyName = LuisHelper.GetNormalizedValueFromEntity(allergy);

                conversation.User.Allergies.Add(allergyName);

                var message = messagesService.Get(MessagesKey.Key.AddAllergy_Added.ToString()).Value;

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
