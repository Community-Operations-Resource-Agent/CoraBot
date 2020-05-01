using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Models;
using System;
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
                default: return string.Empty;
            }
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
