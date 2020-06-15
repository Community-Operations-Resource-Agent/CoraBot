using Microsoft.Bot.Builder;
using Shared.ApiInterface;
using Shared.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    public class CreateGreyshirtMiddleware : IMiddleware
    {
        IApiInterface api;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateGreyshirtMiddleware"/> class.
        /// </summary>
        public CreateGreyshirtMiddleware(IApiInterface api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create the user if they haven't been seen yet.
            var greyshirt = await api.GetGreyshirtFromContext(turnContext);
            if (greyshirt == null)
            {
                greyshirt = new Greyshirt();
                greyshirt.PhoneNumber = Helpers.GetUserToken(turnContext);
                await this.api.Create(greyshirt);
            }

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
