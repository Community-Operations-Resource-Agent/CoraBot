using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Provide
{
    public class MatchDialog : DialogBase
    {
        public static string Name = typeof(MatchDialog).FullName;

        public MatchDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        if (userContext.ProvideMatches.Count == 0)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        var nextMatch = userContext.ProvideMatches.First();
                        userContext.ProvideMatches.RemoveAt(0);

                        var orgUser = await this.api.GetUser(nextMatch.OrgPhoneNumber);
                        var orgNeed = await this.api.GetNeedById(nextMatch.NeedId);

                        var schema = Helpers.GetSchema();
                        var organization = schema.VerifiedOrganizations.FirstOrDefault(o => o.PhoneNumbers.Contains(orgUser.PhoneNumber));

                        // Send the match
                        var message = Phrases.Match.GetMessage(organization.Name, orgNeed.Name, orgNeed.Quantity, orgNeed.Instructions);
                        await Messages.SendAsync(message, turnContext, cancellationToken);

                        if (userContext.ProvideMatches.Count == 0)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        // Ask if they'd like to see another match.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Match.Another(userContext.ProvideMatches.Count) },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return (bool)dialogContext.Result ?
                            await dialogContext.ReplaceDialogAsync(MatchDialog.Name, null, cancellationToken) :
                            await dialogContext.EndDialogAsync(null, cancellationToken);

                    }
                });
            });
        }
    }
}
