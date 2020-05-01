using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    public class TypingMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var typing = turnContext.Activity.CreateReply();
                typing.Type = ActivityTypes.Typing;
                await turnContext.SendActivityAsync(typing);
            }

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
