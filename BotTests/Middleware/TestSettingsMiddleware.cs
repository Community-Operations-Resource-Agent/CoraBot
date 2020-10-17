using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Middleware
{
    public class TestSettingsMiddleware : IMiddleware
    {
        private const string DefaultFromId = "user1";
        private readonly IConfiguration configuration;

        public TestSettingsMiddleware(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            // Read configuration to test specific channel.
            var channelId = configuration.TestChannel();
            if (!string.IsNullOrEmpty(channelId))
            {
                turnContext.Activity.ChannelId = channelId;
            }

            // The default ID causes collisions when multiple tests run at once.
            // Need something unique for each test that runs.
            if (turnContext.Activity.From.Id == DefaultFromId)
            {
                turnContext.Activity.From.Id = Guid.NewGuid().ToString();
            }

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
