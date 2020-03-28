using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
    public class OptionsDialog : DialogBase
    {
        public static string Name = typeof(OptionsDialog).FullName;

        public OptionsDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
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

                        // Offer more keywords.
                        return await dialogContext.PromptAsync(
                            Prompt.KeywordTextPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Greeting.GetOptions(user)
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Restart the master dialog so that it can handle the keyword.
                        return await dialogContext.ReplaceDialogAsync(MasterDialog.Name, null, cancellationToken);
                    }
                });
            });
        }
    }
}
