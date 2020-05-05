using Greyshirt.Dialogs.Missions;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs
{
    public class KeywordDialog : DialogBase
    {
        public static string Name = typeof(KeywordDialog).FullName;

        public KeywordDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Context.Activity.Text.StartsWith(Phrases.Keywords.Accept, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, AcceptMissionDialog.Name, null, cancellationToken);
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
                        // Let the master dialog know whether to continue or not.
                        return await dialogContext.EndDialogAsync((bool)dialogContext.Result, cancellationToken);
                    }
                });
            });
        }
    }
}
