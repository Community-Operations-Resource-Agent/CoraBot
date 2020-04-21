using Bot.Dialogs.Preferences;
using BotTests.Setup;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BotTests.Dialogs.Preferences
{
    [Collection(TestCollectionName)]
    public class TimeDialogTests : DialogTestBase
    {
        public TimeDialogTests(TestFixture fixture) : base(fixture)
        { }

        [Theory]
        [MemberData(nameof(TestTimes))]
        public async Task Update(string currentTime, bool currentTimeIsValid, string reminderTime, bool reminderTimeIsValid)
        {
            var testFlow = CreateTestFlow(TimeDialog.Name)
                .Test("test", Phrases.Preferences.GetCurrentTime)
                .Test(currentTime, currentTimeIsValid ? Phrases.Preferences.GetUpdateTime : Phrases.Preferences.GetCurrentTimeRetry);

            if (currentTimeIsValid)
            {
                var parseResult = DateTime.TryParse(reminderTime, out var reminder);

                testFlow = testFlow.Test(reminderTime, reminderTimeIsValid ?
                    Phrases.Preferences.UpdateTimeUpdated(reminder.ToShortTimeString()) :
                    Phrases.Preferences.GetUpdateTimeRetry);
            }

            await testFlow.StartTestAsync();
        }

        public static IEnumerable<object[]> TestTimes()
        {
            return new List<object[]>()
            {
                // CurrentTime, CurrentTimeIsValid, ReminderTime, ReminderTimeIsValid.
                new object[] { "test", false, "", false },
                new object[] { "12", false, "", false },
                new object[] { "12:15", false, "", false },
                new object[] { "13 pm", false, "", false },
                new object[] { "12:590am", false, "", false },

                new object[] { "12pm", true, "test", false },
                new object[] { "12 pm", true, "12", false },
                new object[] { "12:00pm", true, "12:15", false },
                new object[] { "3:30am", true, "13 pm", false },
                new object[] { "3:30 am", true, "12:590am", false },
                new object[] { "9:15am", true, "8:30am", false },
                new object[] { "9:15 am", true, "8:30 am", false },

                new object[] { "10 am", true, "8am", true },
                new object[] { "10am", true, "8 am", true },
            };
        }
    }
}
