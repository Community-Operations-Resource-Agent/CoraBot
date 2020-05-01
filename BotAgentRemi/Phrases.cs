using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Shared.Models;
using System.Collections.Generic;

namespace BotAgentRemi
{
    public static class Phrases
    {
        public const string ProjectName = "Agent Remi";
        public const string ProjectWebsite = "TeamRubiconUSA.org/AgentRemi";
        public static List<string> ValidChannels = new List<string>() { Channels.Emulator, Channels.Sms };

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

        public static class NewUser
        {
            public static Activity Consent = MessageFactory.Text($"Roger that, just a few questions and then we can get you" +
                $" started with a mission! Rest assured, I'll never share your info with anyone without your permission.");
            public static Activity NoConsent = MessageFactory.Text("Over and out! You can message me any time in the future if you change your mind.");

            public static string ConsentYes = $"Chat with {ProjectName}";
            public static string ConsentNo = $"Cancel";
            public static List<string> ConsentOptions = new List<string> { ConsentYes, ConsentNo };

            public static Activity RegistrationComplete = MessageFactory.Text("That's all the information I need - you're now ready to take on a mission!");
        }

        public static class Options
        {
            public static string NewMission = "I need a mission";
            public static string WhatIsMission = "What's a mission?";
            public static string MoreOptions = "More options";

            public static List<string> List = new List<string> { NewMission, WhatIsMission, MoreOptions };

            public static Activity GetOptions = MessageFactory.Text($"Let me know what you'd like to do. {Shared.Phrases.EnterNumber}");

            public static Activity MissionExplaination = MessageFactory.Text($"Missions are super quick (under an hour), high-impact" +
                $" helpouts. They are opportunities for Greyshirts like you to support people in your community with urgent needs.");

            public static class Extended
            {
                public static string UpdateLocation = "Update your location";
                public static string Enable = $"Enable {ProjectName} to contact you";
                public static string Disable = $"Stop {ProjectName} from contacting you";
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
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thank you for the feedback!");
        }
    }
}
