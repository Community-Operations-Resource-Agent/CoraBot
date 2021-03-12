using Greyshirt.Dialogs.Preferences;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs.NewUser
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
                        var greyshirt = await api.GetGreyshirtFromContext(dialogContext.Context);

                        if (string.Equals(result, Shared.Phrases.NewUser.ConsentNo, StringComparison.OrdinalIgnoreCase))
                        {
                            // Did not consent. Delete their user record.
                            await this.api.Delete(greyshirt);

                            await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("NoConsent", null, turnContext.Activity.Locale)));

                            return await dialogContext.EndDialogAsync(false, cancellationToken);
                        }

                        greyshirt.IsConsentGiven = true;
                        await this.api.Update(greyshirt);

                        await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("Consent", null, turnContext.Activity.Locale)));
                        return await BeginDialogAsync(dialogContext, GreyshirtRegisterDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if ((bool)dialogContext.Result)
                        {
                            // Is not registered already
                            return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                        }
                        return await dialogContext.EndDialogAsync(false, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("RegistrationComplete", null, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(true, cancellationToken);
                    },
                });
            });
        }
    }
}
