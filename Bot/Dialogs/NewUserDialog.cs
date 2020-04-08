using Bot.Dialogs.Preferences;
using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
    public class NewUserDialog : DialogBase
    {
        public static string Name = typeof(NewUserDialog).FullName;

        public NewUserDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Welcome and ask for consent.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Greeting.WelcomeNew },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);

                        if (!(bool)dialogContext.Result)
                        {
                            // Did not consent. Delete their user record.
                            await this.api.Delete(user);

                            await Messages.SendAsync(Phrases.Greeting.NoConsent, dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(false, cancellationToken);
                        }
                        else
                        {
                            user.IsConsentGiven = true;
                            await this.api.Update(user);
                        }

                        await Messages.SendAsync(Phrases.Greeting.Consent, dialogContext.Context, cancellationToken);
                        return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        await Messages.SendAsync(Phrases.Greeting.Registered, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(true, cancellationToken);
                    },
                });
            });
        }
    }
}
