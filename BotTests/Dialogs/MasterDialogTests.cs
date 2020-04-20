using System.Linq;
using System.Threading.Tasks;
using Bot.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Shared;
using Xunit;

namespace BotTests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task InvalidChannel()
        {
            await CreateTestFlow(MasterDialog.Name, channelOverride: Channels.Cortana)
                .Send("test")
                .StartTestAsync();

            // Can't access turnContext before the first turn, so must split the message and response apart.
            await CreateTestFlow(MasterDialog.Name)
                .AssertReply(Phrases.Greeting.InvalidChannel(this.turnContext))
                .StartTestAsync();
        }

        [Fact]
        public async Task NewUser()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("test", StartsWith(Phrases.Greeting.WelcomeNew))
                .StartTestAsync();
        }
    }
}
