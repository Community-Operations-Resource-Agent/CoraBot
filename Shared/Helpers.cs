using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shared
{
    public static class Helpers
    {
        /// <summary>
        /// Twilio is currently the only supported interface and it does not support
        /// "\r\n" from Environment.NewLine. We need to use "\n" instead.
        /// </summary>
        public static string NewLine { get { return "\n"; } }

        /// <summary>
        /// Gets a user token from the turn context.
        /// This will vary based on the channel the message is rec.
        /// </summary>
        public static string GetUserToken(ITurnContext turnContext)
        {
            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator: return turnContext.Activity.From.Id;
                case Channels.Sms: return PhoneNumber.Standardize(turnContext.Activity.From.Id);
                case Channels.Directline: return turnContext.Activity.From.Id;
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Validates the schema. Returns the error if any.
        /// </summary>
        public static string ValidateSchema()
        {
            try
            {
                GetSchema();
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Retrieves the schema.
        /// </summary>
        public static SchemaResponse GetSchema()
        {
            // TODO: Get from Azure
            using (StreamReader reader = new StreamReader("Schema.json"))
            {
                string json = reader.ReadToEnd();
                var schema = JsonConvert.DeserializeObject<SchemaResponse>(json);

                // Standardize all phone numbers.
                foreach (var org in schema.VerifiedOrganizations)
                {
                    for (int numberIndex = 0; numberIndex < org.PhoneNumbers.Count; ++numberIndex)
                    {
                        org.PhoneNumbers[numberIndex] = PhoneNumber.Standardize(org.PhoneNumbers[numberIndex]);
                    }
                }

                return schema;
            }
        }

        /// <summary>
        /// Checks if a resource matches a need
        /// </summary>
        public static bool DoesResourceMatchNeed(Need need, Resource resource)
        {
            if (need == null || resource == null)
            {
                return false;
            }

            return !need.UnopenedOnly || resource.IsUnopened;
        }

        /// <summary>
        /// Looks up a location from a string.
        /// </summary>
        public static async Task<LocationResult> StringToLocation(IConfiguration configuration, string location)
        {
            var url = string.Format(configuration.MapsSearchUrlFormat(), configuration.MapsSubscriptionKey(), location);
            var response = await new HttpClient().GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<LocationApiResponse>(responseContent);
            if (data == null && data.Summary.NumResults == 0)
            {
                return null;
            }

            // Return the first city in the results.
            return data.Results.FirstOrDefault(r => r.EntityType == EntityType.Municipality);
        }

        public static void LogInfo(ILogger log, string text)
        {
            if (log != null)
            {
                log.LogInformation(text);
            }
        }

        public static void LogException(ILogger log, Exception exception)
        {
            if (log != null)
            {
                log.LogError(exception, exception.Message);
            }
        }
    }
}
