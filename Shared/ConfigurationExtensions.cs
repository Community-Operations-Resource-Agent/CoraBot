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
        /// The name of the app setting for the database connection string.
        /// </summary>
        private const string DbModelConnectionStringSettingName = "DbModel";

        /// <summary>
        /// The name of the app setting for the MicrosoftAppId.
        /// </summary>
        private const string MicrosoftAppIdSettingName = "MicrosoftAppId";

        /// <summary>
        /// The name of the app setting for the MicrosoftAppPassword.
        /// </summary>
        private const string MicrosoftAppPasswordSettingName = "MicrosoftAppPassword";

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
        private const string CosmosConversationsCollectionSettingName = "CosmosDb:Conversations:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB users collection.
        /// </summary>
        private const string CosmosUsersCollectionSettingName = "CosmosDb:Users:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB resources collection.
        /// </summary>
        private const string CosmosResourcesCollectionSettingName = "CosmosDb:Resources:Collection";

        /// <summary>
        /// The name of the setting that contains the CosmosDB feedback collection.
        /// </summary>
        private const string CosmosFeedbackCollectionSettingName = "CosmosDb:Feedback:Collection";

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

        public static string DbModelConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString(DbModelConnectionStringSettingName);
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

        public static string CosmosConversationsCollection(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosConversationsCollectionSettingName);
        }

        public static string CosmosUsersCollection(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosUsersCollectionSettingName);
        }

        public static string CosmosResourcesCollection(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosResourcesCollectionSettingName);
        }

        public static string CosmosFeedbackCollection(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(CosmosFeedbackCollectionSettingName);
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
