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
    public class NewMissionDialog : DialogBase
    {
        public static string Name = typeof(NewMissionDialog).FullName;
        private const string FLAG_REPROMPT = "REPROMPT";

        public NewMissionDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        var messageText = dialogContext.Options?.ToString();
                        if (messageText != null && messageText.Equals(FLAG_REPROMPT))
                        {
                            return await dialogContext.NextAsync(new FoundChoice() { Value = "No" }, cancellationToken);
                        }

                        // Demo example
                        var user = await this.api.GetUserFromContext(turnContext);
                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("NewMissionExample", null, turnContext.Activity.Locale)));
                        var choices = new List<Choice>{new Choice("Yes"), new Choice("No") };
                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("AbleToAccept", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);

                        /* Uncomment when using info from database
                        var greyshirt = await this.api.GetGreyshirtFromContext(dialogContext.Context);
                        var userContext = await this.state.GetUserContext(turnContext, cancellationToken);

                        // TODO: this could be configurable.
                        double requestMeters = Units.Miles.ToMeters(50);

                        // Get all users within distance.
                        var usersWithinDistance = await this.api.GetUsersWithinDistance(greyshirt.LocationCoordinates, requestMeters);
                        if (usersWithinDistance.Count > 0)
                        {
                            // Get any missions 
                            foreach (var userWithinDistance in usersWithinDistance)
                            {
                                var missions = await this.api.GetMissionsCreatedByUser(userWithinDistance, isAssigned: false);

                                foreach (var mission in missions)
                                {
                                    userContext.Matches.Add(new Match { PhoneNumber = userWithinDistance.PhoneNumber, MissionId = mission.Id });
                                }
                            }

                            // If there were matches, present them to the user.
                            if (userContext.Matches.Count > 0)
                            {
                                await Messages.SendAsync(Phrases.Match.NumMissions(userContext.Matches.Count), turnContext, cancellationToken);
                                return await BeginDialogAsync(dialogContext, MatchDialog.Name, null, cancellationToken);
                            }

                            await Messages.SendAsync(Phrases.Match.None, turnContext, cancellationToken);
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        return await dialogContext.NextAsync(null, cancellationToken); 
                        */
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        if (string.Equals(result, "No", StringComparison.OrdinalIgnoreCase))
                        {
                            await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("AnotherMissionExample", null, turnContext.Activity.Locale)));
                            var choices = new List<Choice>{new Choice("Yes"), new Choice("No"), new Choice("Done with missions") };
                            return await dialogContext.PromptAsync(
                                Prompt.ChoicePrompt,
                                new PromptOptions()
                                {
                                    Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("AbleToAccept", null, turnContext.Activity.Locale)),
                                    Choices = choices
                                },
                                cancellationToken);
                        }
                        else if (string.Equals(result, "Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, MissionAcceptedDialog.Name, null, cancellationToken);
                        }
                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result == null)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }
                        var result = (dialogContext.Result as FoundChoice).Value;
                        if (string.Equals(result, "No", StringComparison.OrdinalIgnoreCase))
                        {
                            return await dialogContext.ReplaceDialogAsync(Name, FLAG_REPROMPT, cancellationToken);
                        }
                        else if (string.Equals(result, "Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, MissionAcceptedDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, "Done with missions", StringComparison.OrdinalIgnoreCase))
                        {
                            await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("DoneWithMissions", null, turnContext.Activity.Locale)));
                        }
                        return await dialogContext.NextAsync(null, cancellationToken);
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
