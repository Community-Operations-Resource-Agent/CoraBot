using Microsoft.Bot.Builder;
using Shared.ApiInterface;
using Shared.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    public class CreateUserMiddleware : IMiddleware
    {
        IApiInterface api;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserMiddleware"/> class.
        /// </summary>
        public CreateUserMiddleware(IApiInterface api)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Create the user if they haven't been seen yet.
            var user = await api.GetUserFromContext(turnContext);
            if (user == null)
            {
                user = new User();
                user.PhoneNumber = Helpers.GetUserToken(turnContext);
                await this.api.Create(user);
            }

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
