using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
    public class RequestDialog : DialogBase
    {
        public static string Name = typeof(RequestDialog).FullName;

        public RequestDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
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

                        if (string.IsNullOrEmpty(user.Location))
                        {
                            // Prompt for the location.
                            return await dialogContext.PromptAsync(
                                Prompt.LocationTextPrompt,
                                new PromptOptions
                                {
                                    Prompt = Phrases.Request.GetLocation,
                                    RetryPrompt = Phrases.Request.GetLocationRetry
                                },
                                cancellationToken);
                        }

                        // Skip this step.
                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result != null)
                        {
                            var location = (string)dialogContext.Result;
                            var position = await Helpers.LocationToPosition(configuration, location);

                            // Save the location. It was validated by the prompt.
                            var user = await api.GetUser(dialogContext.Context);
                            user.Location = location;
                            user.LocationLatitude = position.Lat;
                            user.LocationLongitude = position.Lon;
                            await this.api.Update(user);
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the categories.
                        var schema = Helpers.GetSchema();
                        List<string> categories = schema.Categories.Select(c => c.Name).ToList();

                        var choices = new List<Choice>();
                        categories.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Request.Categories,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // TODO: what if long delay and the categories changed?
                        // Handle the category not being there.
                        // Maybe in validator.



                        var selectedCategory = ((FoundChoice)dialogContext.Result).Value;

                        // Get the resources in the category.
                        // TODO: validate the selected category first so it is guaranteed to be present.
                        var schema = Helpers.GetSchema();
                        var category = schema.Categories.FirstOrDefault(c => c.Name == selectedCategory);
                        List<string> resources = category.Resources.Select(r => r.Name).ToList();

                        var choices = new List<Choice>();
                        resources.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Request.Resources(selectedCategory),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var selectedResource = ((FoundChoice)dialogContext.Result).Value;

                        // Use their location to make a match
                        await Messages.SendAsync("TODO: Provide matches", turnContext, cancellationToken);

                         return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // End this dialog to pop it off the stack.
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
