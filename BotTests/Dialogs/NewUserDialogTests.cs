﻿using Bot.Dialogs;
using BotTests.Setup;
using Microsoft.Bot.Schema;
using Shared;
using System.Threading.Tasks;
using Xunit;

namespace BotTests.Dialogs
{
    [Collection(TestCollectionName)]
    public class NewUserDialogTests : DialogTestBase
    {
        public NewUserDialogTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ConsentNotGiven()
        {
            await CreateTestFlow(NewUserDialog.Name, userConsentGiven: false)
                .Test("test", StartsWith(Phrases.Greeting.WelcomeNew))
                .Test("2", Phrases.Greeting.NoConsent)
                .StartTestAsync();

            var user = await Api.GetUser(turnContext);
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

            var user = await Api.GetUser(turnContext);
            Assert.NotNull(user);
        }
    }
}
