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
    public class ProvideDialog : DialogBase
    {
        public static string Name = typeof(ProvideDialog).FullName;

        public ProvideDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

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
                            new PromptOptions {
                                Prompt = Phrases.Feedback.GetFeedback
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await this.api.GetUser(dialogContext.Context);

                        // Save the feedback.
                        var feedback = new EntityModel.Feedback();
                        feedback.SenderId = user.Id;
                        feedback.Text = (string)dialogContext.Result;
                        await this.api.Create(feedback);

                        // Send thanks.
                        await Messages.SendAsync(Phrases.Feedback.Thanks, dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(cancellationToken);
                    }
                });
            });
        }
    }
}
