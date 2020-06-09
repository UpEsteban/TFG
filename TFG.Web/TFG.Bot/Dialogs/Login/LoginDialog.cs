using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using TFG.Bot.Resources.Messages;
using TFG.Domain.Shared.Abstractions.Services;

namespace TFG.Bot.Dialogs.Login
{
    public class LoginDialog : BaseDialog
    {
        private const string NamePrompt = "NamePrompt";

        private const string PinPrompt = "PinPrompt";

        private readonly IStatePropertyAccessor<ConversationData> conversationStateAccessor;

        private readonly IStatePropertyAccessor<LoginState> loginStateAccessor;

        private readonly IMessagesService messagesService;

        public LoginDialog(ConversationState conversationState, LoginState state, IMessagesService messagesService)
            : base(nameof(LoginDialog), conversationState, messagesService)
        {
            // Get Conversation State Accessor
            conversationStateAccessor = conversationState.CreateProperty<ConversationData>(nameof(ConversationData));

            loginStateAccessor = conversationState.CreateProperty<LoginState>(nameof(LoginState));

            this.messagesService = messagesService;

            AddDialog(new TextPrompt(NamePrompt, NameValidator));
            AddDialog(new TextPrompt(PinPrompt, PinValidator));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                NameStepAsync,
                PinStepAsync,
                EndStepAsync,
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
            var ask = messagesService.Get(MessagesKey.Key.AskName.ToString()).Value;

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(ask)
            };

            return await stepContext.PromptAsync(NamePrompt, options, cancellationToken);
        }

        private async Task<DialogTurnResult> PinStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var ask = messagesService.Get(MessagesKey.Key.AskPin.ToString()).Value;

            var options = new PromptOptions
            {
                Prompt = MessageFactory.Text(ask)
            };

            return await stepContext.PromptAsync(PinPrompt, options, cancellationToken);
        }

        private async Task<DialogTurnResult> EndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = messagesService.Get(MessagesKey.Key.EndLogin.ToString()).Value;

            await stepContext.Context.SendActivityAsync(message);

            var loginState = await loginStateAccessor.GetAsync(stepContext.Context, () => new LoginState());

            var conversationState = await conversationStateAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            if (conversationState.User != null)
            {
                conversationState.User.Name = loginState.User.Name;
                conversationState.User.PinNumber = loginState.User.PinNumber;
            }
            else
            {
                conversationState.User = loginState.User;
            }

            return await stepContext.EndDialogAsync();
        }

        #region Validators
        private async Task<bool> NameValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Succeeded)
            {
                var loginState = await loginStateAccessor.GetAsync(promptContext.Context, () => new LoginState());

                if (loginState.User != null)
                {
                    loginState.User.Name = promptContext.Recognized.Value;
                }
                else
                {
                    loginState.User = new ServiceContracts.Models.User { Name = promptContext.Recognized.Value };
                }

                var message = messagesService.Get(MessagesKey.Key.ConfirmationName.ToString()).Value;

                await promptContext.Context.SendActivityAsync(string.Format(message, promptContext.Recognized.Value));
            }

            return promptContext.Recognized.Succeeded;
        }

        private async Task<bool> PinValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (promptContext.Recognized.Value.Length > 2)
            {
                var message = messagesService.Get(MessagesKey.Key.ConfirmationPin.ToString()).Value;

                var loginState = await loginStateAccessor.GetAsync(promptContext.Context, () => new LoginState());

                loginState.User.PinNumber = promptContext.Recognized.Value;

                await promptContext.Context.SendActivityAsync(string.Format(message, promptContext.Recognized.Value.Length));
            }

            return promptContext.Recognized.Succeeded;
        }
        #endregion
    }
}
