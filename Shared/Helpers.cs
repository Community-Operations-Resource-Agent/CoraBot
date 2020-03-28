using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Shared
{
    public static class Helpers
    {
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
                default: Debug.Fail("Missing channel type"); return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves the resource schema.
        /// </summary>
        public static ResourceDefinitionsResponse GetSchema()
        {
            // TODO: Get from Azure
            using (StreamReader reader = new StreamReader("Schema.json"))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ResourceDefinitionsResponse>(json);
            }
        }

        /// <summary>
        /// Looks up a position from a location.
        /// </summary>
        public static async Task<LocationPosition> LocationToPosition(IConfiguration configuration, string location)
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
            return data.Results.FirstOrDefault(r => r.EntityType == EntityType.Municipality)?.Position;
        }
    }
}
