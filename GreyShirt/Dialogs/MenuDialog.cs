using Greyshirt.Dialogs.Missions;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs
{
    public class MenuDialog : DialogBase
    {
        public static string Name = typeof(MenuDialog).FullName;

        public MenuDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
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

                            if (string.Equals(result, Phrases.Options.NewMission, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, NewMissionDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.MoreOptions, StringComparison.OrdinalIgnoreCase))
                            {
                                return await BeginDialogAsync(dialogContext, OptionsExtendedDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.WhatIsMission, StringComparison.OrdinalIgnoreCase))
                            {
                                await Messages.SendAsync(Phrases.Options.MissionExplaination, turnContext, cancellationToken);
                            }
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Shared.Phrases.Greeting.AnythingElse },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if ((bool)dialogContext.Result)
                        {
                            return await dialogContext.ReplaceDialogAsync(MenuDialog.Name, null, cancellationToken);
                        }

                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
