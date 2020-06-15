using System.Collections.Generic;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace BotAgentRemi
{
    public static class Phrases
    {
        public static class Options
        {
            public static string Need = "I have a need";
            public static string HowDoesItWork = "How does this work?";
            public static string MoreOptions = "More options";

            public static List<string> List = new List<string> { Need, HowDoesItWork, MoreOptions };

            public static Activity GetOptions = MessageFactory.Text($"How can I help? {Shared.Phrases.EnterNumber}");
            public static Activity HowItWorks = MessageFactory.Text($"Once I know what you need, I'll find someone in your community who's ready to help." +
                $" Team Rubicon runs background checks on each of our Greyshirt volunteers. One and all, they're trusted in their communites and passionate about service.");
        }

        public static class Need
        {
            public static Activity GetPrivacyConsent = MessageFactory.Text($"To help with your need," +
                $" I'll need to share your phone number with my manager at Team Rubicon and the Greyshirt matched to you." +
                $" Don't worry, I'll never share your number with anyone else, and it won't be used for marketing. Your privacy is my priority." +
                $" Is that okay? {Shared.Phrases.EnterNumber}");

            public static Activity NoConsent = MessageFactory.Text("No problem, you can always let me know if you change your mind.");

            public static Activity GetNeed = MessageFactory.Text($"Please send me a description of what you need." +
                $" I'll use this description to connect you with one of our Greyshirt volunteers in your community.");

            public static Activity Complete = MessageFactory.Text($"Great, that's all I need to know! Once I match you to one of our" +
                $" trusted Greyshirts in your community, they will contact you for any additional details so that they can give you a hand.");
        }
    }
}
