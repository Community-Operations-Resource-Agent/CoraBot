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
using System;
using System.Threading;
using System.Threading.Tasks;
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

        protected IConfiguration Configuration => fixture?.Configuration;

        protected IApiInterface Api => fixture?.Api;

        protected DialogTestBase(TestFixture fixture)
        {
            this.fixture = fixture;

            state = StateAccessors.Create();
            dialogs = new DialogSet(state.DialogContextAccessor);

            adapter = new TestAdapter()
                .Use(new TestSettingsMiddleware(fixture.Configuration))
                .Use(new AutoSaveStateMiddleware(state.ConversationState))
                .Use(new TrimIncomingMessageMiddleware())
                .Use(new CreateUserMiddleware(fixture.Api))
                .Use(new TranslationMiddleware(fixture.Api, state, fixture.Translator));

            Prompt.Register(dialogs, fixture.Configuration, fixture.Api);
        }

        protected TestFlow CreateTestFlow(string dialogName, bool userConsentGiven = true)
        {
            return new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                this.turnContext = turnContext;
                this.cancellationToken = cancellationToken;

                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    // Initialize the dialog context.
                    DialogContext dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                    // Make sure this channel is supported.
                    if (!Phrases.ValidChannels.Contains(turnContext.Activity.ChannelId))
                    {
                        await Messages.SendAsync(Phrases.Greeting.InvalidChannel(turnContext), turnContext, cancellationToken);
                        return;
                    }

                    // Create the master dialog.
                    var masterDialog = new MasterDialog(state, dialogs, Api, Configuration);

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
                        await state.ClearUserContext(dialogContext.Context, cancellationToken);

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

        private async Task InitUser(bool userConsentGiven)
        {
            var user = await Api.GetUser(turnContext);
            user.IsConsentGiven = userConsentGiven;
            await Api.Update(user);
        }
    }
}
