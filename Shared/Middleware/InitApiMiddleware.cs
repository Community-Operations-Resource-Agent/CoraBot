using Microsoft.Bot.Builder;
using Shared.ApiInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    public class InitApiMiddleware : IMiddleware
    {
        IApiInterface api;

        public InitApiMiddleware(IApiInterface api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            await this.api.Init();

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
