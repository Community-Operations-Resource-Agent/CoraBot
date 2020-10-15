using Bot.Dialogs.Preferences;
using BotTests.Setup;
using Shared;
using Shared.Models.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BotTests.Dialogs.Preferences
{
    [Collection(TestCollectionName)]
    public class DaysDialogTests : DialogTestBase
    {
        public DaysDialogTests(TestFixture fixture) : base(fixture)
        {
        }

        [Theory]
        [MemberData(nameof(TestDays))]
        public async Task Update(string reminderDays, bool reminderDaysIsValid)
        {
            DayFlagsHelpers.FromString(reminderDays, ",", out var dayFlags);

            await CreateTestFlow(DaysDialog.Name)
                .Test("test", Phrases.Preferences.GetUpdateDays)
                .Test(reminderDays, reminderDaysIsValid ? Phrases.Preferences.UpdateDaysUpdated(dayFlags) : Phrases.Preferences.GetUpdateDaysRetry)
                .StartTestAsync();

            if (reminderDaysIsValid)
            {
                var user = await Api.GetUser(turnContext);
                Assert.Equal(user.ReminderFrequency, dayFlags);
            }
        }

        public static IEnumerable<object[]> TestDays()
        {
            return new List<object[]>()
            {
                // ReminderDays, ReminderDaysIsValid
                new object[] { "test", false },
                new object[] { "mon,tues", false },

                new object[] { "Sa,Su", true },
                new object[] { "M,W,F", true },
                new object[] { "m,t,w,th,f", true },
                new object[] { "M,T,W,Th,F,Sa,Su", true },
                new object[] { "Saturday", true },
                new object[] { "Monday,Wednesday", true },
                new object[] { "Weekdays", true },
                new object[] { "weekends", true },
                new object[] { "Everyday", true },
            };
        }
    }
}
