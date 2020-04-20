using System.Threading.Tasks;
using Bot.Dialogs;
using Bot.Dialogs.Preferences;
using Microsoft.Bot.Schema;
using Shared;
using Xunit;

namespace BotTests.Dialogs.Preferences
{
    public class LocationDialogTests : DialogTestBase
    {
        [Fact]
        public async Task LocationRetry()
        {
            await CreateTestFlow(LocationDialog.Name)
                .Test("test", Phrases.Preferences.GetLocation)
                .Test("abcdefghij", Phrases.Preferences.GetLocationRetry)
                .StartTestAsync();
        }

        [Fact]
        public async Task LocationIncorrect()
        {
            await CreateTestFlow(LocationDialog.Name)
                .Test("test", Phrases.Preferences.GetLocation)
                .Send("Seattle")
                .StartTestAsync();

            var user = await this.api.GetUser(this.turnContext);

            await CreateTestFlow(LocationDialog.Name)
                .AssertReply(StartsWith(Phrases.Preferences.GetLocationConfirm(user.Location)))
                .Test("2", Phrases.Preferences.GetLocation)
                .StartTestAsync();
        }

        [Fact]
        public async Task LocationCorrect()
        {
            await CreateTestFlow(LocationDialog.Name)
                .Test("test", Phrases.Preferences.GetLocation)
                .Send("Seattle")
                .StartTestAsync();

            var user = await this.api.GetUser(this.turnContext);

            await CreateTestFlow(LocationDialog.Name)
                .AssertReply(StartsWith(Phrases.Preferences.GetLocationConfirm(user.Location)))
                .Test("1", Phrases.Preferences.LocationUpdated)
                .StartTestAsync();
        }
    }
}
