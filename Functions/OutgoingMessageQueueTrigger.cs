using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Functions
{
    public static class OutgoingMessageQueueTrigger
    {
        [FunctionName(nameof(OutgoingMessageQueueTrigger))]
        public static async void Run([QueueTrigger(OutgoingMessageQueueHelpers.QueueName, Connection = "AzureWebJobsStorage")]
            OutgoingMessageQueueData queueData, ILogger log, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                await DoWork(configuration, queueData, log);
            }
            catch (Exception e)
            {
                Helpers.LogException(log, e);
                throw e;
            }
        }

        public static async Task DoWork(IConfiguration configuration, OutgoingMessageQueueData queueData, ILogger log = null)
        {
            if (string.IsNullOrEmpty(queueData.PhoneNumber) ||
                string.IsNullOrEmpty(queueData.Message) ||
                !PhoneNumber.IsValid(queueData.PhoneNumber))
            {
                return;
            }

            MicrosoftAppCredentials.TrustServiceUrl(configuration.ServiceUrl());
            var creds = new MicrosoftAppCredentials(configuration.MicrosoftAppId(), configuration.MicrosoftAppPassword());
            var credentialProvider = new SimpleCredentialProvider(creds.MicrosoftAppId, creds.MicrosoftAppPassword);
            var adapter = new BotFrameworkAdapter(credentialProvider);
            var botAccount = new ChannelAccount() { Id = configuration.BotPhoneNumber() };
            var userAccount = new ChannelAccount() { Id = PhoneNumber.Standardize(queueData.PhoneNumber) };
            var convoAccount = new ConversationAccount(id: userAccount.Id);
            var convo = new ConversationReference(null, userAccount, botAccount, convoAccount, configuration.ChannelId(), configuration.ServiceUrl());

            await adapter.ContinueConversationAsync(creds.MicrosoftAppId, convo, async (context, token) =>
            {
                await context.SendActivityAsync(queueData.Message);
            }, new CancellationToken());
        }
    }
}
