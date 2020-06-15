using Microsoft.Extensions.Configuration;
using System;

namespace Shared
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// The name of the app setting for the environment.
        /// </summary>
        private const string EnvironmentSettingName = "Environment";

        /// <summary>
        /// The name of the app setting for the MicrosoftAppId.
        /// </summary>
        private const string MicrosoftAppIdSettingName = "MicrosoftAppId";

        /// <summary>
        /// The name of the app setting for the MicrosoftAppPassword.
        /// </summary>
        private const string MicrosoftAppPasswordSettingName = "MicrosoftAppPassword";

        /// <summary>
        /// The name of the app setting for the azure storage connection string.
        /// </summary>
        private const string AzureWebJobsStorageSettingName = "AzureWebJobsStorage";

        /// <summary>
        /// The name of the app setting for the Greyshirt azure storage connection string.
        /// </summary>
        private const string GreyshirtAzureWebJobsStorageSettingName = "GreyshirtAzureWebJobsStorage";

        /// <summary>
        /// The name of the setting that contains the CosmosDB endpoint.
        /// </summary>
        private const string CosmosEndpointSettingName = "CosmosDb:Endpoint";

        /// <summary>
        /// The name of the setting that contains the CosmosDB key.
        /// </summary>
        private const string CosmosKeySettingName = "CosmosDb:Key";

        /// <summary>
        /// The name of the setting that contains the CosmosDB database.
        /// </summary>
        private const string CosmosDatabaseSettingName = "CosmosDb:Database";

        /// <summary>
        /// The name of the setting that contains the CosmosDB conversations collection.
        /// </summary>
        private const string CosmosConversationsContainerSettingName = "CosmosDb:Conversations:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB users collection.
        /// </summary>
        private const string CosmosUsersContainerSettingName = "CosmosDb:Users:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB missions collection.
        /// </summary>
        private const string CosmosMissionsContainerSettingName = "CosmosDb:Missions:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB feedback collection.
        /// </summary>
        private const string CosmosFeedbackContainerSettingName = "CosmosDb:Feedback:Collection";

        /// <summary>
        /// The name of the setting that contains the ApplicationInsights config.
        /// </summary>
        private const string LuisAppIdSettingName = "Luis:AppId";

        /// <summary>
        /// The name of the setting that contains the LUIS subscription key.
        /// </summary>
        private const string LuisSubscriptionKeySettingName = "Luis:SubscriptionKey";

        /// <summary>
        /// The name of the setting that contains the LUIS endpoint URL.
        /// </summary>
        private const string LuisEndpointUrlSettingName = "Luis:EndpointUrl";

        /// <summary>
        /// The name of the setting that contains the maps subscription key.
        /// </summary>
        private const string MapsSubscriptionKeySettingName = "Maps:SubscriptionKey";

        /// <summary>
        /// The name of the setting that contains the maps search URL format.
        /// </summary>
        private const string MapsSearchUrlFormatSettingName = "Maps:SearchUrlFormat";

        /// <summary>
        /// The name of the setting that contains the maps subscription key.
        /// </summary>
        private const string TranslationSubscriptionKeySettingName = "Translation:SubscriptionKey";

        /// <summary>
        /// The name of the setting that contains the maps search URL format.
        /// </summary>
        private const string TranslationUrlFormatSettingName = "Translation:UrlFormat";

        /// <summary>
        /// The name of the setting that contains the bot service URL.
        /// </summary>
        private const string ServiceUrlSettingName = "ServiceUrl";

        /// <summary>
        /// The name of the setting that contains the bot channel ID.
        /// </summary>
        private const string ChannelIdSettingName = "ChannelId";

        /// <summary>
        /// The name of the setting that contains the bot phone number.
        /// </summary>
        private const string BotPhoneNumberSettingName = "BotPhoneNumber";

        /// <summary>
        /// The name of the setting that contains the channel ID for tests.
        /// </summary>
        private const string TestChannelSettingName = "TestChannel";

        public static string Environment(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(EnvironmentSettingName);
        }

        public static string MicrosoftAppId(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(MicrosoftAppIdSettingName);
        }

        public static string MicrosoftAppPassword(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(MicrosoftAppPasswordSettingName);
        }

        public static string AzureWebJobsStorage(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(AzureWebJobsStorageSettingName);
        }

        public static string GreyshirtAzureWebJobsStorage(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(GreyshirtAzureWebJobsStorageSettingName);
        }

        public static string CosmosEndpoint(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosEndpointSettingName);
        }

        public static string CosmosKey(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosKeySettingName);
        }

        public static string CosmosDatabase(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosDatabaseSettingName);
        }

        public static string CosmosConversationsContainer(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosConversationsContainerSettingName);
        }

        public static string CosmosUsersContainer(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosUsersContainerSettingName);
        }

        public static string CosmosMissionsContainer(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosMissionsContainerSettingName);
        }

        public static string CosmosFeedbackContainer(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosFeedbackContainerSettingName);
        }

        public static string LuisAppId(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(LuisAppIdSettingName);
        }

        public static string LuisSubscriptionKey(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(LuisSubscriptionKeySettingName);
        }

        public static string LuisEndpointUrl(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(LuisEndpointUrlSettingName);
        }

        public static string MapsSubscriptionKey(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(MapsSubscriptionKeySettingName);
        }

        public static string MapsSearchUrlFormat(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(MapsSearchUrlFormatSettingName);
        }

        public static string TranslationSubscriptionKey(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(TranslationSubscriptionKeySettingName);
        }

        public static string TranslationUrlFormat(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(TranslationUrlFormatSettingName);
        }

        public static string ServiceUrl(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(ServiceUrlSettingName);
        }

        public static string ChannelId(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(ChannelIdSettingName);
        }

        public static string BotPhoneNumber(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(BotPhoneNumberSettingName);
        }

        public static string TestChannel(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(TestChannelSettingName);
        }

        public static bool IsProduction(this IConfiguration configuration)
        {
            return string.Equals(configuration.Environment(), "Production", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
