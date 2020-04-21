using System;
using System.Threading;
using System.Threading.Tasks;
using Bot.Dialogs;
using Bot.Middleware;
using Bot.State;
using BotTests.Setup;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using Xunit;

namespace BotTests.Dialogs
{
    public abstract class DialogTestBase
    {
        protected const string TestCollectionName = "TestCollection";

        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly TestAdapter adapter;

        protected readonly TestFixture fixture;

        protected ITurnContext turnContext;
        protected CancellationToken cancellationToken;

        protected IConfiguration Configuration { get { return fixture?.Configuration; } }
        protected IApiInterface Api { get { return fixture?.Api; } }

        protected DialogTestBase(TestFixture fixture)
        {
            this.fixture = fixture;

            this.state = StateAccessors.Create();
            this.dialogs = new DialogSet(state.DialogContextAccessor);

            this.adapter = new TestAdapter()
                .Use(new TestSettingsMiddleware(fixture.Configuration))
                .Use(new AutoSaveStateMiddleware(state.ConversationState))
                .Use(new TrimIncomingMessageMiddleware())
                .Use(new CreateUserMiddleware(fixture.Api))
                .Use(new TranslationMiddleware(fixture.Api, this.state, fixture.Translator));

            Prompt.Register(this.dialogs, fixture.Configuration, fixture.Api);
        }

        protected TestFlow CreateTestFlow(string dialogName, bool userConsentGiven = true)
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
                    var masterDialog = new MasterDialog(this.state, this.dialogs, this.Api, this.Configuration);

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

                        // Tests must init the user once there is a turn context.
                        await InitUser(userConsentGiven);

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

        async Task InitUser(bool userConsentGiven)
        {
            var user = await this.Api.GetUser(this.turnContext);
            user.IsConsentGiven = userConsentGiven;
            await this.Api.Update(user);
        }
    }
}
