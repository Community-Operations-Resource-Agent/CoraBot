using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotAgentRemi.Dialogs.Provide;
using BotAgentRemi.Dialogs.Request;
using BotAgentRemi.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;

namespace BotAgentRemi.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

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

                        var user = await api.GetUser(dialogContext.Context);
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

                        var user = await api.GetUser(dialogContext.Context);
                        var phoneNumber = PhoneNumber.Standardize(user.PhoneNumber);
                        var schema = Helpers.GetSchema();
                        bool isVerifiedOrganization = schema.VerifiedOrganizations.Any(o => o.PhoneNumbers.Contains(phoneNumber));

                        // Prompt for an option.
                        var choices = new List<Choice>();
                        Phrases.Options.GetOptionsList(isVerifiedOrganization).ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Options.GetOptions,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result is FoundChoice choice)
                        {
                            var result = choice.Value;

                            if (string.Equals(result, Phrases.Options.Request, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, RequestDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.Provide, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, ProvideDialog.Name, null, cancellationToken);
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
                        return await dialogContext.ReplaceDialogAsync(MasterDialog.Name, null, cancellationToken);
                    }
                });
            });
        }
    }
}
