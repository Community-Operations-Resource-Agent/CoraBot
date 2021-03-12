using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Remy.Dialogs.Need;
using Remy.Dialogs.NewUser;
using Remy.State;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remy.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Clear the user context when a new converation begins.
                        await this.state.ClearUserContext(dialogContext.Context, cancellationToken);


                        /*
                        // TODO
                        await Messages.SendAsync("I'm not yet up and running. Coming soon!", turnContext, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                        */


                        var user = await api.GetUserFromContext(dialogContext.Context);
                        if (!user.IsConsentGiven)
                        {
                            return await BeginDialogAsync(dialogContext, NewUserDialog.Name, null, cancellationToken);
                        }

                        // Skip this step.
                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // The new user flow can result in no consent. If so, end the conversation.
                        if (dialogContext.Result is bool didConsent && !didConsent)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        // Prompt for an option.
                        // TODO: Get from LG
                        var choices = new List<Choice>();
                        Phrases.Options.List.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("GetOptions", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result is FoundChoice choice)
                        {
                            var result = choice.Value;

                            if (string.Equals(result, Phrases.Options.FoodAssistance, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, FoodAssistanceDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.FoodBank, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, FoodBankDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.ShoppingDelivery, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, ShoppingDeliveryDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.AskAQuestion, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, QnAMakerDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.MoreOptions, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, OptionsExtendedDialog.Name, null, cancellationToken);
                            }
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("AnythingElse", null, turnContext.Activity.Locale)) },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if ((bool)dialogContext.Result)
                        {
                            return await dialogContext.ReplaceDialogAsync(MasterDialog.Name, null, cancellationToken);
                        }

                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("Goodbye", null, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
