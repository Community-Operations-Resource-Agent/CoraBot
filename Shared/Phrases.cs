using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;

namespace Shared
{
    public static class Phrases
    {
        public const string ProjectName = "CORA";
        public const string ProjectDescription = "the Covid-19 Operations Resource Agent";
        public static List<string> ValidChannels = new List<string>() { Channels.Emulator, Channels.Sms };

        public static class Exceptions
        {
            public static string Error = $"Sorry, it looks like something went wrong";
        }

        public static class Keywords
        {
            public const string Provide = "provide";
            public const string Request = "request";
            public const string Feedback = "feedback";
            public const string Options = "options";

            public static List<string> List = new List<string>() { Provide, Request, Options };

            public static string HowToProvide = $"Send \"{Provide}\" to meet a need";
            public static string HowToRequest = $"Send \"{Request}\" to request a need";
            public static string HowToOptions = $"Send \"{Options}\" for more options";
            public static string HowToFeedback = $"Send \"{Feedback}\" to provide feedback";
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thanks for the feedback!");
        }

        public static class Greeting
        {
            public static string Welcome = "Welcome back!";
            public static string WelcomeNew = $"Welcome to {ProjectName}, {ProjectDescription}!";

            public static Activity InvalidChannel(ITurnContext turnContext)
            {
                return MessageFactory.Text($"Channel \"{turnContext.Activity.ChannelId}\" is not yet supported");
            }

            public static Activity GetKeywords()
            {
                string greeting = "Here's what you can do:" +
                                  "- " + Keywords.HowToRequest + Environment.NewLine +
                                  "- " + Keywords.HowToProvide + Environment.NewLine +
                                  "- " + Keywords.HowToOptions;

                return MessageFactory.Text(greeting);
            }
        }

        public static class Request
        {
            public static Activity GetLocation = MessageFactory.Text("In what city are you looking for resources? (City, State)");
            public static Activity GetLocationRetry = MessageFactory.Text($"Oops, I couldn't find that location. Please try again...");
            public static Activity Categories = MessageFactory.Text("Which category of resources are you looking for?");
            
            public static Activity Resources(string category)
            {
                return MessageFactory.Text($"Which type of {category.ToLower()} are you looking for?");
            }
        }
    }
}
