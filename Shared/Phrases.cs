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
            public const string Update = "update";
            public static List<string> List = new List<string>() { Update };
        }

        public static class Options
        {
            public static string Request = "Request a need";
            public static string Provide = "Meet a need or update availability";
            public static string MoreOptions = "More options";

            public static class Extended
            {
                public static string UpdateLocation = "Update your location";
                public static string ChangeDays = $"Change the days that {ProjectName} will contact you for updates";
                public static string ChangeTime = $"Change the time that {ProjectName} will contact you for updates";
                public static string Enable = $"Enable {ProjectName} to contact you";
                public static string Disable = $"Stop {ProjectName} from contacting you";
                public static string Feedback = "Provide feedback";
            }
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

            public static Activity GetOptions = MessageFactory.Text("Here's what you can do (enter a number)");
            public static Activity GetOptionsExtended = MessageFactory.Text("Here are some more options (enter a number)");

            public static List<string> GetOptionsList()
            {
                return new List<string> { Options.Request, Options.Provide, Options.MoreOptions };
            }

            public static List<string> GetOptionsExtendedList(User user)
            {
                var list = new List<string> { Options.Extended.UpdateLocation, Options.Extended.ChangeDays, Options.Extended.ChangeTime };
                list.Add(user.ContactEnabled ? Options.Extended.Disable : Options.Extended.Enable);
                list.Add(Options.Extended.Feedback);
                return list;
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

            private const string PreferenceUpdated = "Your contact preference has been updated";

            public static Activity GetLocation = MessageFactory.Text("Where are you located? (City,State,Country)");
            public static Activity GetLocationRetry = MessageFactory.Text($"Oops, I couldn't find that location. Please try again...");
            public static Activity LocationUpdated = MessageFactory.Text("Your location has been updated!");

            public static Activity GetCurrentTime = MessageFactory.Text($"What time is it for you currently? This is to determine your timezone. {GetCurrentTimeExample}");
            public static Activity GetCurrentTimeRetry = MessageFactory.Text($"Oops, the format is {GetCurrentTimeFormat}. {GetCurrentTimeExample}");

            public static Activity GetUpdateTime = MessageFactory.Text($"Which hour of the day would you like to be contacted? {GetUpdateTimeExample}");
            public static Activity GetUpdateTimeRetry = MessageFactory.Text($"Oops, the format is {GetUpdateTimeFormat}. {GetUpdateTimeExample}");

            public static Activity GetUpdateDays = MessageFactory.Text($"Which days of the week would you like to be contacted? {GetUpdateDaysExample}");
            public static Activity GetUpdateDaysRetry = MessageFactory.Text($"Oops, the format is {GetUpdateDaysFormat}. {GetUpdateDaysExample}");

            public static Activity UpdateTimeUpdated(string time)
            {
                return MessageFactory.Text($"{PreferenceUpdated} to {time}!");
            }

            public static Activity UpdateDaysUpdated(DayFlags days)
            {
                return MessageFactory.Text($"{PreferenceUpdated} to {days.ToString()}!");
            }

            public static Activity ContactEnabledUpdated(bool contactEnabled)
            {
                return MessageFactory.Text($"{PreferenceUpdated}!");
            }
        }

        public static class Provide
        {
            public static Activity Categories = MessageFactory.Text("Which category of resources are you able to provide? (enter a number)");
            public static Activity GetQuantity = MessageFactory.Text("What quantity of this resource do you have available?");
            public static Activity GetAvailable = MessageFactory.Text("Do you still have this resource available?");
            public static Activity CompleteUpdate = MessageFactory.Text("Thank you for the update!");

            public static Activity Resources(string category)
            {
                return MessageFactory.Text($"Which type of {category.ToLower()} are you able to provide? (enter a number)");
            }

            public static Activity CompleteCreate(User user)
            {
                var text = "Thank you for making your resources available to the community! " +
                    "You may be contacted by someone if they have a need that matches your resources.";
                
                if (user.ContactEnabled)
                {
                    text += $" I will contact you {user.ReminderFrequency.ToString()} to update your availability. This frequency can be customized from the options menu";
                }
                else
                {
                    text += $" Your contact preference is disabled, so please make sure to update your availability if it changes - thank you!";
                }

                return MessageFactory.Text(text);
            }
        }

        public static class Request
        {
            public static Activity Categories = MessageFactory.Text("Which category of resource are you looking for?");
            
            public static Activity Resources(string category)
            {
                return MessageFactory.Text($"Which type of {category.ToLower()} are you looking for?");
            }

            public static Activity Match(UserResourcePair match, double distance)
            {
                if (match == null)
                {
                    return MessageFactory.Text($"Unfortunately I was unable to find a match at this time. Please check back soon!");
                }

                var text = $"The closest match I found is { Math.Round(distance, MidpointRounding.AwayFromZero) } miles from you.";

                if (match.Resource.HasQuantity)
                {
                    text += $" It looks like there {(match.Resource.Quantity == 1 ? "is" : "are")} {match.Resource.Quantity} available.";
                }

                text += $" Here is the contact phone number: {match.User.PhoneNumber}";

                return MessageFactory.Text(text);
            }
        }
    }
}
