using Bot.Dialogs.Preferences;
using BotTests.Setup;
using Microsoft.Bot.Schema;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BotTests.Dialogs.Preferences
{
    [Collection(TestCollectionName)]
    public class LocationDialogTests : DialogTestBase
    {
        public LocationDialogTests(TestFixture fixture) : base(fixture)
        {
        }

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

            var user = await Api.GetUser(turnContext);

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

            var user = await Api.GetUser(turnContext);

            await CreateTestFlow(LocationDialog.Name)
                .AssertReply(StartsWith(Phrases.Preferences.GetLocationConfirm(user.Location)))
                .Test("1", Phrases.Preferences.LocationUpdated)
                .StartTestAsync();
        }
    }
}
