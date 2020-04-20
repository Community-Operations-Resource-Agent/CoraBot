using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Middleware
{
    public class TestChannelMiddleware : IMiddleware
    {
        private IConfiguration configuration;

        public TestChannelMiddleware(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            // Read configuration to test specific channel.
            var channelId = this.configuration.TestChannel();
            if (!string.IsNullOrEmpty(channelId))
            {
                turnContext.Activity.ChannelId = channelId;
            }

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
