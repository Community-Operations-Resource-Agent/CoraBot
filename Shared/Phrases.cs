using EntityModel;
using EntityModel.Helpers;
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
        public const string ProjectDescription = "the COVID-19 Operations Resource Agent";
        public static List<string> ValidChannels = new List<string>() { Channels.Emulator, Channels.Sms };

        public static class Exceptions
        {
            public static string Error = $"Sorry, it looks like something went wrong";
        }

        public static class Keywords
        {
            public const string Provide = "provide";
            public const string Request = "request";
            public const string Options = "options";
            public const string Location = "location";
            public const string Enable = "enable";
            public const string Disable = "disable";
            public const string Days = "days";
            public const string Time = "time";
            public const string Feedback = "feedback";

            public static List<string> List = new List<string>() { Provide, Request, Options, Location, Enable, Disable, Days, Time, Feedback };

            public static string HowToProvide = $"Send \"{Provide}\" to meet a need";
            public static string HowToRequest = $"Send \"{Request}\" to request a need";
            public static string HowToOptions = $"Send \"{Options}\" for more options";
            public static string HowToUpdateLocation = $"Send \"{Location}\" to update your location";
            public static string HowToFeedback = $"Send \"{Feedback}\" to provide feedback";
            public static string HowToEnable = $"Send \"{Enable}\" to allow {ProjectName} to contact you";
            public static string HowToDisable = $"Send \"{Disable}\" to stop {ProjectName} from contacting you";
            public static string HowToChangeDays = $"Send \"{Days}\" to change the days that {ProjectName} will contact you for updates";
            public static string HowToChangeTime = $"Send \"{Time}\" to change the time that {ProjectName} will contact you for updates";
        }

        public static class Feedback
        {
            public static Activity GetFeedback = MessageFactory.Text($"What would you like to let the {ProjectName} team know?");
            public static Activity Thanks = MessageFactory.Text("Thanks for the feedback!");
        }

        public static class Greeting
        {
            public static string Welcome = "Welcome back!";
            public static string WelcomeNew = $"Hi, I'm {ProjectName}, {ProjectDescription}!";

            public static Activity InvalidChannel(ITurnContext turnContext)
            {
                return MessageFactory.Text($"Channel \"{turnContext.Activity.ChannelId}\" is not yet supported");
            }

            public static Activity GetKeywords()
            {
                string greeting = "Here's what you can do:" + Helpers.NewLine +
                                  "- " + Keywords.HowToRequest + Helpers.NewLine +
                                  "- " + Keywords.HowToProvide + Helpers.NewLine +
                                  "- " + Keywords.HowToOptions;

                return MessageFactory.Text(greeting);
            }

            public static Activity GetOptions(User user)
            {
                string greeting = "Here's are some more options:" + Helpers.NewLine +
                                  "- " + Keywords.HowToUpdateLocation + Helpers.NewLine +
                                  "- " + (user.ContactEnabled ? Keywords.HowToDisable : Keywords.HowToEnable) + Helpers.NewLine +
                                  "- " + Keywords.HowToChangeDays + Helpers.NewLine +
                                  "- " + Keywords.HowToChangeTime + Helpers.NewLine +
                                  "- " + Keywords.HowToFeedback;

                return MessageFactory.Text(greeting);
            }
        }

        public static class Preferences
        {
            private const string GetCurrentTimeFormat = "\"h:mm am/pm\"";
            private const string GetUpdateTimeFormat = "\"h am/pm\"";
            private const string GetUpdateDaysFormat = "\"M,T,W,Th,F,Sa,Su\"";

            private const string GetCurrentTimeExample = "You can say things like \"8:30 am\" or \"12:15 pm\"";
            private const string GetUpdateTimeExample = "You can say things like \"8 am\" or \"12 pm\"";
            private const string GetUpdateDaysExample = "You can say things like \"M,W,F\", \"weekdays\", \"weekends\", or \"everyday\"";

            private const string Updated = "Your contact preference has been updated";

            public static Activity GetCurrentTime = MessageFactory.Text($"What time is it for you currently? This is to determine your timezone. {GetCurrentTimeExample}");
            public static Activity GetCurrentTimeRetry = MessageFactory.Text($"Oops, the format is {GetCurrentTimeFormat}. {GetCurrentTimeExample}");

            public static Activity GetUpdateTime = MessageFactory.Text($"Which hour of the day would you like to be contacted? {GetUpdateTimeExample}");
            public static Activity GetUpdateTimeRetry = MessageFactory.Text($"Oops, the format is {GetUpdateTimeFormat}. {GetUpdateTimeExample}");

            public static Activity GetUpdateDays = MessageFactory.Text($"Which days of the week would you like to be contacted? {GetUpdateDaysExample}");
            public static Activity GetUpdateDaysRetry = MessageFactory.Text($"Oops, the format is {GetUpdateDaysFormat}. {GetUpdateDaysExample}");

            public static Activity UpdateTimeUpdated(string time)
            {
                return MessageFactory.Text($"{Updated} to {time}!");
            }

            public static Activity UpdateDaysUpdated(DayFlags days)
            {
                return MessageFactory.Text($"{Updated} to {days.ToString()}!");
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"{Updated}! " + (contactEnabled ? Keywords.HowToDisable : Keywords.HowToEnable));
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
