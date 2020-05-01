using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Shared
{
    public static class Phrases
    {
        public static string EnterNumber = "(enter a number)";
        public static string None = "None of these";

        public static class Exceptions
        {
            public static string Error = $"Sorry, it looks like something went wrong." +
                $" If this continues to happen, send \"{Keywords.Reset}\" to reset the conversation";
        }

        public static class Keywords
        {
            public static string Reset = "Reset";
        }

        public static class Preferences
        {
            private const string PreferenceUpdated = "Your contact preference has been updated";

            public static Activity GetLocation = MessageFactory.Text("Where are you located? (enter City, State)");
            public static Activity GetLocationRetry = MessageFactory.Text($"Oops, I couldn't find that location. Please try again...");
            public static Activity LocationUpdated = MessageFactory.Text("Your location has been updated!");

            public static Activity GetLocationConfirm(string location)
            {
                return MessageFactory.Text($"I matched your city to \"{location}\". Is this correct? {EnterNumber}");
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"{PreferenceUpdated}!");
            }
        }
    }
}
