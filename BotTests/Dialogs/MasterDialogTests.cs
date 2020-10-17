using Bot.Dialogs;
using BotTests.Setup;
using Microsoft.Bot.Schema;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BotTests.Dialogs
{
    [Collection(TestCollectionName)]
    public class MasterDialogTests : DialogTestBase
    {
        public MasterDialogTests(TestFixture fixture) : base(fixture)
        {
        }

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

        [Fact]
        public async Task UpdateKeyword()
        {
            await CreateTestFlow(MasterDialog.Name)
                .Test("test", StartsWith(Phrases.Options.GetOptions))
                .Test("2", StartsWith(Phrases.Options.Extended.GetOptions))
                .Test(Phrases.Keywords.Update, StartsWith(Phrases.Options.GetOptions))
                .StartTestAsync();
        }
    }
}
