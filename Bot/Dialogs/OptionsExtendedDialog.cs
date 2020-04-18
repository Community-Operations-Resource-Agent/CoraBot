using Bot.Dialogs.Preferences;
using Bot.State;
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

namespace Bot.Dialogs
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
                        Phrases.Options.Extended.GetOptionsList(user).ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Options.Extended.GetOptions,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = ((FoundChoice)dialogContext.Result).Value;

                        if (string.Equals(result, Phrases.Options.Extended.UpdateLocation, StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Options.Extended.ChangeDays, StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, DaysDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Options.Extended.ChangeTime, StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, TimeDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Options.Extended.Enable, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(result, Phrases.Options.Extended.Disable, StringComparison.OrdinalIgnoreCase))
                        {
                            // Enable/disable contact.
                            var enable = string.Equals(result, Phrases.Options.Extended.Enable, StringComparison.OrdinalIgnoreCase);

                            var user = await this.api.GetUser(dialogContext.Context);
                            if (user.ContactEnabled != enable)
                            {
                                user.ContactEnabled = enable;
                                await this.api.Update(user);
                            }

                            await Messages.SendAsync(Phrases.Preferences.ContactEnabledUpdated(user.ContactEnabled), dialogContext.Context, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Options.Extended.Language, StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, LanguageDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Options.Extended.Feedback, StringComparison.OrdinalIgnoreCase))
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
