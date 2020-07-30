using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs
{
    public class FeedbackDialog : DialogBase
    {
        public static string Name = typeof(FeedbackDialog).FullName;

        public FeedbackDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Prompt for feedback.
                        return await dialogContext.PromptAsync(
                            Prompt.TextPrompt,
                            new PromptOptions { Prompt = Shared.Phrases.Feedback.GetFeedback },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var greyshirt = await this.api.GetGreyshirtFromContext(dialogContext.Context);

                        var feedback = new Feedback();
                        feedback.CreatedById = greyshirt.Id;
                        feedback.Text = (string)dialogContext.Result;
                        await this.api.Create(feedback);

                        await Messages.SendAsync(Shared.Phrases.Feedback.Thanks, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
