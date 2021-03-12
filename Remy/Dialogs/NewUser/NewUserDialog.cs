using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Remy.Dialogs.Preferences;
using Remy.State;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remy.Dialogs.NewUser
{
    public class NewUserDialog : DialogBase
    {
        public static string Name = typeof(NewUserDialog).FullName;

        public NewUserDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Welcome and ask for consent.
                        // TODO: Get from LG
                        var choices = new List<Choice>();
                        Shared.Phrases.NewUser.ConsentOptions.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("WelcomeNew", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        var user = await api.GetUserFromContext(dialogContext.Context);

                        if (string.Equals(result, Shared.Phrases.NewUser.ConsentNo, StringComparison.OrdinalIgnoreCase))
                        {
                            // Did not consent. Delete their user record.
                            await this.api.Delete(user);

                            await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("NoConsent", null, turnContext.Activity.Locale)));
                            return await dialogContext.EndDialogAsync(false, cancellationToken);
                        }

                        user.IsConsentGiven = true;
                        await this.api.Update(user);

                        await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("Consent", null, turnContext.Activity.Locale)));
                        return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.EndDialogAsync(true, cancellationToken);
                    },
                });
            });
        }
    }
}
