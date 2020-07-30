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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs.Missions
{
    public class MatchDialog : DialogBase
    {
        public static string Name = typeof(MatchDialog).FullName;

        public MatchDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        if (userContext.Matches.Count == 0)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        var nextMatch = userContext.Matches.First();
                        var needUser = await this.api.GetUserFromPhoneNumber(nextMatch.PhoneNumber);
                        var mission = await this.api.GetMissionById(nextMatch.MissionId);

                        var choices = new List<Choice>();
                        Phrases.Match.MatchOptions.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Match.OfferMission(mission.Description, needUser.Location),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        var match = userContext.Matches.First();
                        userContext.Matches.RemoveAt(0);

                        var result = (dialogContext.Result as FoundChoice).Value;

                        if (string.Equals(result, Phrases.Match.AcceptMission, StringComparison.OrdinalIgnoreCase))
                        {
                            // TODO: Assign to them.
                            await Messages.SendAsync(Phrases.Match.Accepted(match.PhoneNumber), turnContext, cancellationToken);
                        }

                        if (userContext.Matches.Count == 0)
                        {
                            await Messages.SendAsync(Phrases.Match.NoMore, turnContext, cancellationToken);
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Match.Another(userContext.Matches.Count) },
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
