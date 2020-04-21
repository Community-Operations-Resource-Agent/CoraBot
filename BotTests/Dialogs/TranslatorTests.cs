using System.Threading.Tasks;
using Bot.Dialogs;
using BotTests.Setup;
using Microsoft.Bot.Schema;
using Xunit;

namespace BotTests.Dialogs
{
    [Collection(TestCollectionName)]
    public class TranslatorTests : DialogTestBase
    {
        public TranslatorTests(TestFixture fixture) : base(fixture)
        { }

        [Fact]
        public async Task Spanish()
        {
            await CreateTestFlow(MasterDialog.Name, userConsentGiven: false)
                .Send("hola")
                .AssertReplyContains(string.Empty)
                .StartTestAsync();

            var user = await this.Api.GetUser(this.turnContext);
            Assert.Equal("es", user.Language);
        }

        [Fact]
        public async Task French()
        {
            await CreateTestFlow(MasterDialog.Name, userConsentGiven: false)
                .Send("bonjour")
                .AssertReplyContains(string.Empty)
                .StartTestAsync();

            var user = await this.Api.GetUser(this.turnContext);
            Assert.Equal("fr", user.Language);
        }
    }
}
