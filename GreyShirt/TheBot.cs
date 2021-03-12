using Greyshirt.Dialogs;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;
        private readonly IApiInterface api;
        private readonly MultiLanguageLG lgGenerator;
        private readonly IConfiguration configuration;

        public TheBot(IConfiguration configuration, StateAccessors state, CosmosInterface api, MultiLanguageLG lgGenerator)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.api = api ?? throw new ArgumentNullException(nameof(api));

            this.lgGenerator = lgGenerator ?? throw new ArgumentNullException(nameof(lgGenerator));

            // Register prompts.
            Prompt.Register(this.dialogs, this.configuration, this.api);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Establish context for our dialog from the turn context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Make sure this channel is supported.
                if (!Shared.Phrases.ValidChannels.Contains(turnContext.Activity.ChannelId))
                {
                    await Messages.SendAsync(Shared.Phrases.Greeting.InvalidChannel(turnContext), turnContext, cancellationToken);
                    return;
                }

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration, this.lgGenerator);

                // If the user sends the keyword, clear the dialog stack and start a new session.
                if (string.Equals(turnContext.Activity.Text, Shared.Phrases.Keywords.Reset, StringComparison.OrdinalIgnoreCase))
                {
                    await dialogContext.CancelAllDialogsAsync(cancellationToken);
                }
                else if (string.Equals(turnContext.Activity.Text, Shared.Phrases.Keywords.Nuke, StringComparison.OrdinalIgnoreCase))
                {
                    // Remove the user and all their data. Should only be used for testing.
                    await this.api.ResetUser(turnContext);
                    await dialogContext.CancelAllDialogsAsync(cancellationToken);
                }

                // Attempt to continue any existing conversation.
                DialogTurnResult result = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);

                // Start a new conversation if there isn't one already.
                if (result.Status == DialogTurnStatus.Empty)
                {
                    await masterDialog.BeginDialogAsync(dialogContext, MasterDialog.Name, null, cancellationToken);
                }
            }
        }
    }
}
