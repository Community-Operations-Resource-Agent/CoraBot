using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Greyshirt
{
    public static class Phrases
    {
        public static class Keywords
        {
            public static string Accept = "Accept";

            public static bool IsKeyword(string text)
            {
                return text.StartsWith(Accept, StringComparison.InvariantCultureIgnoreCase) ||
                    Shared.Phrases.Keywords.List.Any(k => string.Equals(text, k, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static class NewUser
        {
            public static Activity RegistrationComplete = MessageFactory.Text("That's all the information I need - you're now ready to take on a mission!");
        }

        public static class Register
        {
            public static Activity GetIsRegistered = MessageFactory.Text($"Are you already registered as a Greyshirt? {Shared.Phrases.EnterNumber}");
            public static Activity HowToRegister = MessageFactory.Text($"No problem, let's get you registered as a Greyshirt." +
                $" Go to TeamRubiconUSA.org/engage. Select Join Up and then follow the onscreen instructions.");
            public static Activity GetNumberNew = MessageFactory.Text($"Once you finish registering, reply to me by sending your new Greyshirt number");
            public static Activity GetNumberNewRepeat = MessageFactory.Text($"What is your Greyshirt number?");
            public static Activity GetNumberExisting = MessageFactory.Text($"That's what I like to hear! What is your Greyshirt number?");
            public static Activity GetNumberConfirm = MessageFactory.Text($"Great! We're almost ready to get started!");
        }

        public static class Options
        {
            public static string NewMission = "I need a mission";
            public static string WhatIsMission = "What's a mission?";
            public static string AskAQuestion = "Ask a question";
            public static string MoreOptions = "More options";

            public static List<string> List = new List<string> { NewMission, WhatIsMission, AskAQuestion, MoreOptions };

            public static Activity GetOptions = MessageFactory.Text($"Let me know what you'd like to do. {Shared.Phrases.EnterNumber}");
            public static Activity MissionExplaination = MessageFactory.Text($"Missions are super quick (under an hour), high-impact" +
                $" helpouts. They are opportunities for Greyshirts like you to support people in your community with urgent needs.");
        }

        public static class Match
        {
            public static Activity None = MessageFactory.Text("Unfortunately I don't have any missions near you at this time." +
                " Check back in again soon, and I will also let you know if any new missions pop up near you!");

            public static Activity NoMore = MessageFactory.Text("That's all the missions I have near you at this time." +
                " Check back in again soon, and I will also let you know if any new missions pop up near you!");

            public static string AcceptMission = "I'm on it!";
            public static string DeclineMission = "I'll pass on this one";
            public static List<string> MatchOptions = new List<string> { AcceptMission, DeclineMission };

            public static Activity NumMissions(int num)
            {
                return MessageFactory.Text($"I have {num} {(num == 1 ? "mission" : "missions")} available near you!");
            }

            public static Activity OfferMission(string instructions, string location)
            {
                return MessageFactory.Text($"Here's a mission in {location} - \"{instructions}\". Would you like to accept this mission? {Shared.Phrases.EnterNumber}");
            }

            public static Activity Accepted(string phoneNumber)
            {
                return MessageFactory.Text("That's what I was hoping to hear! This mission is now assigned to you." +
                    $" You can coordinate by calling or texting them at {phoneNumber}." +
                    $" I'll check back in with you periodically to see how it is coming along!");
            }

            public static Activity Another(int remaining)
            {
                return MessageFactory.Text($"Would you like me to send you another mission near you? I have {remaining} more available. {Shared.Phrases.EnterNumber}");
            }
        }

        public static class Need
        {
            public static Activity InvalidFormat = MessageFactory.Text("Sorry, I wasn't able to understand that format." +
                $" Please try sending it again as \"{Keywords.Accept} XXXXX\"");
            public static Activity InvalidId = MessageFactory.Text("Sorry, I wasn't able to find that ID." +
                $" Please try sending it again as \"{Keywords.Accept} XXXXX\"");
            public static Activity AlreadyAssigned = MessageFactory.Text("Sorry, it looks like this mission was already accepted" +
                " by another Greyshirt. I will continue to let you know if any new missions pop up near you!");

            public static string Notification(string location, Mission mission)
            {
                return $"Hey there Greyshirt, a new mission has been received in {location} - \"{mission.Description}\"." +
                    $" If you would like to take on this mission, reply \"{Keywords.Accept} {mission.ShortId}\".";
            }
        }
    }
}
