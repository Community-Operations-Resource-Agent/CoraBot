using System.Collections.Generic;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

using Shared.Models;
using Shared.Translation;

namespace Shared
{
    public static class Phrases
    {
        public const string ProjectName = "Agent Remi";
        public const string ProjectWebsite = "TeamRubiconUSA.org/AgentRemi";
        public static List<string> ValidChannels = new List<string>() { Channels.Emulator, Channels.Sms };

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
            public static string Nuke = "Nuke";

            public static List<string> List = new List<string> { Reset, Nuke };
        }

        public static class NewUser
        {
            public static Activity Consent = MessageFactory.Text($"Roger that, just a few questions and then we can get you" +
                $" started! Rest assured, I'll never share your info with anyone without your permission.");
            public static Activity NoConsent = MessageFactory.Text("You can message me any time in the future if you change your mind. Over and out!");

            public static string ConsentYes = $"Chat with {ProjectName}";
            public static string ConsentNo = $"Cancel";
            public static List<string> ConsentOptions = new List<string> { ConsentYes, ConsentNo };
        }

        public static class Greeting
        {
            public static Activity Welcome = MessageFactory.Text("Welcome back!");
            public static Activity WelcomeNew = MessageFactory.Text($"Welcome, {ProjectName} here!" +
                $" I'm an agent for Team Rubicon, helping people across the United States get the food and resources they need." +
                $" Message and data rates apply. Would you like to continue? {EnterNumber}");

            public static Activity AnythingElse = MessageFactory.Text($"Is there anything else I can help with today? {Shared.Phrases.EnterNumber}");
            public static Activity Goodbye = MessageFactory.Text($"Over and out!");

            public static Activity InvalidChannel(ITurnContext turnContext)
            {
                return MessageFactory.Text($"Channel \"{turnContext.Activity.ChannelId}\" is not yet supported");
            }
        }

        public static class OptionsExtended
        {
            public static string UpdateLocation = "Update your location";
            public static string UpdateLanguage = "Change your language";
            public static string Enable = $"Enable {ProjectName} to contact you";
            public static string Disable = $"Stop {ProjectName} from contacting you";
            public static string Feedback = "Provide feedback";
            public static string GoBack = "None of these";

            public static Activity GetOptions = MessageFactory.Text($"Let me know what you'd like to do. {EnterNumber}");

            public static List<string> GetOptionsList(User user, Translator translator)
            {
                var list = new List<string> { UpdateLocation };

                if(translator.IsConfigured)
                {
                    list.Add(UpdateLanguage);
                }

                list.Add(Feedback);
                list.Add(user.ContactEnabled ? Disable : Enable);
                list.Add(GoBack);
                return list;
            }
        }

        public static class Preferences
        {
            private const string PreferenceUpdated = "Your contact preference has been updated";

            public static Activity GetLocation = MessageFactory.Text("Where are you located? (enter City, State)");
            public static Activity GetLocationRetry = MessageFactory.Text($"Oops, I couldn't find that location. Please try again...");
            public static Activity LocationUpdated = MessageFactory.Text("Your location has been updated!");

            public static Activity GetLanguage = MessageFactory.Text($"Say \"hello\" in the language you prefer and I will switch to that language for you");
            public static Activity LanguageUpdated = MessageFactory.Text($"Your language has been updated!");

            public static Activity GetLocationConfirm(string location)
            {
                return MessageFactory.Text($"I matched your city to \"{location}\". Is this correct? {EnterNumber}");
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"{PreferenceUpdated}!");
            }
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thank you for the feedback!");
        }
    }
}
