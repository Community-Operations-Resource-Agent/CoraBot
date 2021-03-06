﻿using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using Shared.Translation;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Preferences
{
    public class LanguageDialog : DialogBase
    {
        public static string Name = typeof(LanguageDialog).FullName;

        public LanguageDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration)
        {
        }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.PromptAsync(
                            Prompt.TextPrompt,
                            new PromptOptions { Prompt = Phrases.Preferences.GetLanguage },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Use the original response in the channel data (set by the middleware)
                        // because the response is already translated to the bot language at this point.
                        if (dialogContext.Context.Activity.ChannelData is string originalResponse)
                        {
                            var translator = new Translator(configuration);
                            var response = await translator.TranslateToDataAsync(originalResponse, Translator.DefaultLanguage, cancellationToken);

                            var user = await api.GetUser(dialogContext.Context);
                            user.Language = response.DetectedLanguage.Language ?? Translator.DefaultLanguage;
                            await api.Update(user);
                        }

                        await Messages.SendAsync(Phrases.Preferences.LanguageUpdated, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
