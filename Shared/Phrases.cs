using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Shared.Models;
using System.Collections.Generic;

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
        }

        public static class Greeting
        {
            public static Activity Welcome = MessageFactory.Text("Welcome back!");
            public static Activity WelcomeNew = MessageFactory.Text($"Welcome, {ProjectName} here!" +
                $" I'm a bot for Team Rubicon, helping people across the United States get the food and resources they need." +
                $" Message and data rates apply. Would you like to continue? {Shared.Phrases.EnterNumber}");

            public static Activity InvalidChannel(ITurnContext turnContext)
            {
                return MessageFactory.Text($"Channel \"{turnContext.Activity.ChannelId}\" is not yet supported");
            }
        }

        public static class OptionsExtended
        {
            public static string UpdateLocation = "Update your location";
            public static string Enable = $"Enable {Shared.Phrases.ProjectName} to contact you";
            public static string Disable = $"Stop {Shared.Phrases.ProjectName} from contacting you";
            public static string Feedback = "Provide feedback";
            public static string GoBack = "Go back to the main menu";

            public static Activity GetOptions = MessageFactory.Text($"Let me know what you'd like to do. {Shared.Phrases.EnterNumber}");

            public static List<string> GetOptionsList(User user)
            {
                var list = new List<string> { UpdateLocation };
                list.Add(user.ContactEnabled ? Disable : Enable);
                list.AddRange(new string[] { Feedback, GoBack });
                return list;
            }
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
