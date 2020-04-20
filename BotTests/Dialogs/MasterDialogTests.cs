using System.Threading.Tasks;
using Bot.Dialogs;
using Microsoft.Bot.Schema;
using Shared;
using Xunit;

namespace BotTests.Dialogs
{
    public class MasterDialogTests : DialogTestBase
    {
        [Fact]
        public async Task NewUser()
        {
            await CreateTestFlow(MasterDialog.Name, userConsentGiven: false)
                .Test("test", StartsWith(Phrases.Greeting.WelcomeNew))
                .StartTestAsync();
        }

        [Fact]
        public async Task ExistingUser()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("test", StartsWith(Phrases.Options.GetOptions))
                .StartTestAsync();
        }
    }
}
