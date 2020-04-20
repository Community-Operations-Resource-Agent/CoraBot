using System.Threading.Tasks;
using Bot.Dialogs;
using Microsoft.Bot.Schema;
using Shared;
using Xunit;

namespace BotTests.Dialogs
{
    public class NewUserDialogTests : DialogTestBase
    {
        [Fact]
        public async Task ConsentNotGiven()
        {
            await CreateTestFlow(NewUserDialog.Name, userConsentGiven: false)
                .Test("test", StartsWith(Phrases.Greeting.WelcomeNew))
                .Test("2", Phrases.Greeting.NoConsent)
                .StartTestAsync();

            var user = await this.api.GetUser(this.turnContext);
            Assert.Null(user);
        }

        [Fact]
        public async Task ConsentGiven()
        {
            await CreateTestFlow(NewUserDialog.Name, userConsentGiven: false)
                .Test("test", StartsWith(Phrases.Greeting.WelcomeNew))
                .Test("1", Phrases.Greeting.Consent)
                .AssertReply(Phrases.Preferences.GetLocation)
                .StartTestAsync();

            var user = await this.api.GetUser(this.turnContext);
            Assert.NotNull(user);
        }
    }
}
