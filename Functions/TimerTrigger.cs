using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Storage;
using Shared.Translation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceProviderTriggers
{
    public static class TimerTrigger
    {
        [FunctionName(nameof(TimerTrigger))]
        public static async Task Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation($"Executed at: {DateTime.Now}");

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                await DoWork(configuration, log);
            }
            catch(Exception e)
            {
                Helpers.LogException(log, e);
                throw e;
            }
        }

        public static async Task DoWork(IConfiguration configuration, ILogger log = null)
        {
            //var api = new CosmosInterface(configuration);
            //var translator = new Translator(configuration);

            //// The reminder message is static, so cache any translations to limit API calls.
            //var translationCache = new Dictionary<string, string>();

            //// Get the current day.
            //var day = DayFlagsHelpers.CurrentDay();

            //// Get all users.
            //// TODO: this should likely be broken up so that it doesn't exceed the function time limit.
            //var users = await api.GetUsers();
            //Helpers.LogInfo(log, $"Found {users.Count} users");

            //var queueHelper = new OutgoingMessageQueueHelpers(configuration.AzureWebJobsStorage());

            //foreach (var user in users)
            //{
            //    // Check if the user should be reminded today.
            //    if (!user.ContactEnabled || !user.ReminderFrequency.HasFlag(day))
            //    {
            //        continue;
            //    }

            //    // Check if the user should be reminded at this time.
            //    // Special case: use 6pm UTC (10am PST) if their reminder time isn't set.
            //    DateTime userReminderTime = string.IsNullOrEmpty(user.ReminderTime) ?
            //        DateTime.Parse("6:00pm") : DateTime.Parse(user.ReminderTime);

            //    // Using a 5 minute window in case the function triggers slightly early or late.
            //    if (Math.Abs((DateTime.UtcNow - userReminderTime).TotalMinutes) > 5)
            //    {
            //        continue;
            //    }

            //    var message = Phrases.Greeting.RemindToUpdate;

            //    // Check if the user's language is already cached.
            //    if (translationCache.TryGetValue(user.Language, out var translation))
            //    {
            //        message = translation;
            //    }
            //    else
            //    {
            //        // Translate the message if necessary.
            //        if (translator.IsConfigured && user.Language != Translator.DefaultLanguage)
            //        {
            //            message = await translator.TranslateAsync(message, user.Language);
            //        }

            //        // Cache the message
            //        translationCache.Add(user.Language, message);
            //    }

            //    var data = new OutgoingMessageQueueData
            //    {
            //        PhoneNumber = user.PhoneNumber,
            //        Message = message
            //    };

            //    await queueHelper.Enqueue(data);
            //}
        }
    }
}
