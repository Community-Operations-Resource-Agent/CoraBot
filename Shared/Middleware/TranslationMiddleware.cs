using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Shared.ApiInterface;
using Shared.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class TranslationMiddleware : IMiddleware
    {
        IApiInterface api;
        Translator translator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMiddleware"/> class.
        /// </summary>
        public TranslationMiddleware(IApiInterface api, Translator translator)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.translator = translator ?? throw new ArgumentNullException(nameof(translator));
        }

        /// <summary>
        /// Processes an incoming activity.
        /// </summary>
        /// <param name="turnContext">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            await HandleIncomingMessage(turnContext, cancellationToken);

            turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
            {
                await HandleOutgoingMessage(activities, newContext, cancellationToken);
                return await nextSend();
            });

            turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
            {
                await HandleOutgoingMessage(new List<Activity> { activity }, newContext, cancellationToken);
                return await nextUpdate();
            });

            // Invoke the next middleware.
            await next(cancellationToken).ConfigureAwait(false);
        }

        private async Task HandleIncomingMessage(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (!this.translator.IsConfigured || turnContext.Activity.Type != ActivityTypes.Message)
            {
                return;
            }

            // Store the original incoming text so that it is still available after translation.
            turnContext.Activity.ChannelData = turnContext.Activity.Text;

            var user = await api.GetUserFromContext(turnContext);

            // Translate messages sent from the user to the default language.
            if (user.Language != Translator.DefaultLanguage)
            {
                // For the first message we need to detect the user's language and store it.
                if (string.IsNullOrEmpty(user.Language))
                {
                    var result = await this.translator.TranslateToDataAsync(turnContext.Activity.Text, Translator.DefaultLanguage, cancellationToken);
                    turnContext.Activity.Text = result?.Translations?.FirstOrDefault()?.Text;

                    user.Language = result?.DetectedLanguage?.Language ?? Translator.DefaultLanguage;
                    await this.api.Update(user);
                }
                else
                {
                    await TranslateAsync(turnContext.Activity, Translator.DefaultLanguage, cancellationToken);
                }
            }
        }

        private async Task HandleOutgoingMessage(List<Activity> activities, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (!this.translator.IsConfigured)
            {
                return;
            }

            var user = await api.GetUserFromContext(turnContext);

            // User can be null if they do not consent and their record is deleted.
            if (user == null)
            {
                return;
            }

            // Translate messages sent to the user to the user's language.
            if (user.Language != Translator.DefaultLanguage)
            {
                List<Task> tasks = new List<Task>();
                foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                {
                    tasks.Add(TranslateAsync(currentActivity, user.Language, cancellationToken));
                }

                if (tasks.Any())
                {
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
        }

        private async Task TranslateAsync(Activity activity, string targetLocale, CancellationToken cancellationToken)
        {
            activity.Text = await this.translator.TranslateAsync(activity.Text, targetLocale, cancellationToken);
        }
    }
}