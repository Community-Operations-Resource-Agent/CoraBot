using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Provide
{
    public class UpdateDialog : DialogBase
    {
        public static string Name = typeof(UpdateDialog).FullName;

        public UpdateDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
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
                        var resources = await this.api.GetResourcesForUser(user);

                        var choices = new List<Choice>();
                        resources.ForEach(r => choices.Add(new Choice { Value = r.Name }));
                        choices.Add(new Choice { Value = Phrases.None });

                        // Prompt for the which resource they want to update.
                        return await dialogContext.PromptAsync(
                            Prompt.UpdateResourcePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Provide.Update.GetResource,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Choice was validated in case the schema changed.
                        var selectedChoice = (FoundChoice)dialogContext.Result;
                        var selectedResource = selectedChoice.Value;

                        if (selectedResource == Phrases.None)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        var user = await api.GetUser(dialogContext.Context);
                        var resources = await this.api.GetResourcesForUser(user);
                        var resource = resources[selectedChoice.Index];

                        // Store the resource in the user context.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.Category = resource.Category;
                        userContext.Resource = resource.Name;

                        return await BeginDialogAsync(dialogContext, UpdateResourceDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Check if they want to update another resource.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Provide.Update.Another },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if ((bool)dialogContext.Result)
                        {
                            return await dialogContext.ReplaceDialogAsync(UpdateDialog.Name, null, cancellationToken);
                        }

                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
