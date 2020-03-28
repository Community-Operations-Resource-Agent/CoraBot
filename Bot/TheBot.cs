using System;
using System.Threading;
using System.Threading.Tasks;
using Bot.Dialogs;
using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;

namespace Bot
{
    public class TheBot : IBot
    {
        private readonly StateAccessors state;
        private readonly DialogSet dialogs;
        private readonly IApiInterface api;
        private readonly IConfiguration configuration;

        public TheBot(IConfiguration configuration, StateAccessors state, EfInterface api)
        {
            this.configuration = configuration;

            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.api = api ?? throw new ArgumentNullException(nameof(api));

            // Register prompts.
            Prompt.Register(this.dialogs, this.configuration);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Establish context for our dialog from the turn context.
                DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                // Make sure this channel is supported.
                if (!Phrases.ValidChannels.Contains(turnContext.Activity.ChannelId))
                {
                    await Messages.SendAsync(Phrases.Greeting.InvalidChannel(turnContext), turnContext, cancellationToken);
                    return;
                }

                // Create the master dialog.
                var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

                /*
                // If the user sends the update keyword, clear the dialog stack and start a new update.
                if (string.Equals(turnContext.Activity.Text, Phrases.Keywords.Update, StringComparison.OrdinalIgnoreCase))
                {
                    await dialogContext.CancelAllDialogsAsync(cancellationToken);
                }
                */

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
