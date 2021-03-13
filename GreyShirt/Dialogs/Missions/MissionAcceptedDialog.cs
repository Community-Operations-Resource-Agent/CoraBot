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

namespace Greyshirt.Dialogs.Missions
{
    public class MissionAcceptedDialog : DialogBase
    {
        public static string Name = typeof(MissionAcceptedDialog).FullName;

        public MissionAcceptedDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        var choices = new List<Choice>{new Choice("Mission complete"), new Choice("I'm unable to complete my mission"), new Choice("I don't know what to do") };
                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("MissionAcceptedDialog", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        if (string.Equals(result, "Mission complete", StringComparison.OrdinalIgnoreCase))
                        {
                            await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("MissionCompleteMessage", null, turnContext.Activity.Locale)));
                        }
                        else if (string.Equals(result, "I'm unable to complete my mission", StringComparison.OrdinalIgnoreCase))
                        {
                            await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("UnableToCompleteMessage", null, turnContext.Activity.Locale)));
                        }
                        else if (string.Equals(result, "I don't know what to do", StringComparison.OrdinalIgnoreCase))
                        {
                            await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("DontKnowWhatToDoMessage", null, turnContext.Activity.Locale)));
                        }
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
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
