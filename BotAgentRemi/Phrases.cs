using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Shared.Models;
using System.Collections.Generic;

namespace BotAgentRemi
{
    public static class Phrases
    {
        public static class NewUser
        {
            public static Activity Consent = MessageFactory.Text($"Roger that, just a few questions and then we can get you" +
                $" started with a mission! Rest assured, I'll never share your info with anyone without your permission.");
            public static Activity NoConsent = MessageFactory.Text("Over and out! You can message me any time in the future if you change your mind.");

            public static string ConsentYes = $"Chat with {Shared.Phrases.ProjectName}";
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
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {Shared.Phrases.ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thank you for the feedback!");
        }
    }
}
