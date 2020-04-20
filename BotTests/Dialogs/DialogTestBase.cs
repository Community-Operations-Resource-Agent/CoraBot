using System;
using System.Threading;
using System.Threading.Tasks;
using Bot.Dialogs;
using Bot.Middleware;
using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using Shared.Translation;
using Xunit;

namespace BotTests.Dialogs
{
    public abstract class DialogTestBase : IAsyncLifetime
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly IApiInterface api;
        protected readonly TestAdapter adapter;
        private readonly IConfiguration configuration;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        protected DialogTestBase()
        {
            this.configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .Build();

            this.state = StateAccessors.Create();
            this.dialogs = new DialogSet(state.DialogContextAccessor);
            this.api = new CosmosInterface(configuration);

            var translator = new Translator(this.configuration);

            this.adapter = new TestAdapter()
                .Use(new TestChannelMiddleware(this.configuration))
                .Use(new AutoSaveStateMiddleware(state.ConversationState))
                .Use(new TrimIncomingMessageMiddleware())
                .Use(new CreateUserMiddleware(this.api))
                .Use(new TranslationMiddleware(this.api, this.state, translator));

            Prompt.Register(this.dialogs, this.configuration, this.api);
        }

        public async Task InitializeAsync()
        {
            await this.api.Init();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        protected TestFlow CreateTestFlow(string dialogName, string channelOverride = null)
        {
            return new TestFlow(this.adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    // Initialize the dialog context.
                    DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

                    // Make sure this channel is supported.
                    if (!Phrases.ValidChannels.Contains(turnContext.Activity.ChannelId))
                    {
                        await Messages.SendAsync(Phrases.Greeting.InvalidChannel(turnContext), turnContext, cancellationToken);
                        return;
                    }

                    // Create the master dialog.
                    var masterDialog = new MasterDialog(this.state, this.dialogs, this.api, this.configuration);

                    // If the user sends the update keyword, clear the dialog stack and start a new update.
                    if (string.Equals(turnContext.Activity.Text, Phrases.Keywords.Update, StringComparison.OrdinalIgnoreCase))
                    {
                        dialogName = MasterDialog.Name;
                        await dialogContext.CancelAllDialogsAsync(cancellationToken);
                    }

                    // Attempt to continue any existing conversation.
                    DialogTurnResult result = await masterDialog.ContinueDialogAsync(dialogContext, cancellationToken);

                    // Start a new conversation if there isn't one already.
                    if (result.Status == DialogTurnStatus.Empty)
                    {
                        // Clear the user context when a new conversation begins.
                        await this.state.ClearUserContext(dialogContext.Context, cancellationToken);

                        // Difference for tests here is beginning the given dialog instead of master so that individual dialog flows can be tested.
                        await masterDialog.BeginDialogAsync(dialogContext, dialogName, null, cancellationToken);
                    }
                }
            });
        }

        protected Action<IActivity> StartsWith(IMessageActivity expected)
        {
            return receivedActivity =>
            {
                // Validate the received activity.
                var received = receivedActivity.AsMessageActivity();
                Assert.NotNull(received);
                Assert.StartsWith(expected.Text, received.Text);
            };
        }
    }
}
