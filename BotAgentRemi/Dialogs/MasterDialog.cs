using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using System.Threading;
using System.Threading.Tasks;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using BotAgentRemi.State;
using BotAgentRemi.Dialogs.NewUser;
using Shared;

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


                        // TODO
                        await Messages.SendAsync("I'm not yet up and running. Coming soon!", turnContext, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);



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

                        // Prompt for an option.
                        var choices = new List<Choice>();
                        Phrases.Options.List.ForEach(s => choices.Add(new Choice { Value = s }));

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

                            if (string.Equals(result, "TODO", StringComparison.OrdinalIgnoreCase))
                            {
                                // TODO
                                //return await BeginDialogAsync(dialogContext, NewMissionDialog.Name, null, cancellationToken);
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
