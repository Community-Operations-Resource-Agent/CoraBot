using BotAgentRemi.Dialogs.Preferences;
using BotAgentRemi.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotAgentRemi.Dialogs
{
    public class OptionsExtendedDialog : DialogBase
    {
        public static string Name = typeof(OptionsExtendedDialog).FullName;

        public OptionsExtendedDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);

                        // Prompt for an option.
                        var choices = new List<Choice>();
                        Shared.Phrases.OptionsExtended.GetOptionsList(user).ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Shared.Phrases.OptionsExtended.GetOptions,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = ((FoundChoice)dialogContext.Result).Value;

                        if (string.Equals(result, Shared.Phrases.OptionsExtended.UpdateLocation, StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Shared.Phrases.OptionsExtended.Enable, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(result, Shared.Phrases.OptionsExtended.Disable, StringComparison.OrdinalIgnoreCase))
                        {
                            // Enable/disable contact.
                            var enable = string.Equals(result, Shared.Phrases.OptionsExtended.Enable, StringComparison.OrdinalIgnoreCase);

                            var user = await this.api.GetUser(dialogContext.Context);
                            if (user.ContactEnabled != enable)
                            {
                                user.ContactEnabled = enable;
                                await this.api.Update(user);
                            }

                            await Messages.SendAsync(Shared.Phrases.Preferences.ContactEnabledUpdated(user.ContactEnabled), dialogContext.Context, cancellationToken);
                        }
                        else if (string.Equals(result, Shared.Phrases.OptionsExtended.Feedback, StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, FeedbackDialog.Name, null, cancellationToken);
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
