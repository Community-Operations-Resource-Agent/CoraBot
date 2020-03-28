using Bot.State;
using EntityModel.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Preferences
{
    public class DaysDialog : DialogBase
    {
        public static string Name = typeof(DaysDialog).FullName;

        public DaysDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get which days they want to be contacted.
                        return await dialogContext.PromptAsync(
                            Prompt.DaysPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Preferences.GetUpdateDays,
                                RetryPrompt = Phrases.Preferences.GetUpdateDaysRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the result. This was already validated by the prompt.
                        DayFlagsHelpers.FromString((string)dialogContext.Result, ",", out DayFlags dayFlags);

                        // Update the user's preference.
                        var user = await api.GetUser(dialogContext.Context);
                        user.ReminderFrequency = dayFlags;
                        await this.api.Update(user);

                        // Send a confirmation message.
                        await Messages.SendAsync(Phrases.Preferences.UpdateDaysUpdated(dayFlags), dialogContext.Context, cancellationToken);

                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
