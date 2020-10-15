using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public static class Messages
    {
        public static async Task SendAsync(string message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await SendAsync(MessageFactory.Text(message), turnContext, cancellationToken);
        }

        public static async Task SendAsync(Activity message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(message, cancellationToken);
        }
    }
}
