using Bot.Dialogs.Preferences;
using Bot.State;
using EntityModel.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Request
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
                        // Get the categories.
                        var schema = Helpers.GetSchema();
                        List<string> categories = schema.Categories.Select(c => c.Name).ToList();

                        if (categories.Count == 1)
                        {
                            // No need to ask for a single category.
                            var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                            userContext.Category = schema.Categories.First().Name;

                            // Skip this step.
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        var choices = new List<Choice>();
                        categories.ForEach(c => choices.Add(new Choice { Value = c }));
                        choices.Add(new Choice { Value = Phrases.None });

                        return await dialogContext.PromptAsync(
                            Prompt.CategoryPrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Provide.GetCategory,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var schema = Helpers.GetSchema();
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        if (dialogContext.Result is FoundChoice)
                        {
                            // Choice was validated in case the schema changed.
                            var selectedCategory = ((FoundChoice)dialogContext.Result).Value;

                            if (selectedCategory == Phrases.None)
                            {
                                return await dialogContext.EndDialogAsync(null, cancellationToken);
                            }

                            // Store the category in the user context.
                            userContext.Category = selectedCategory;
                        }

                        // Get the resources in the category.
                        var category = schema.Categories.FirstOrDefault(c => c.Name == userContext.Category);
                        List<string> resources = category.Resources.Select(r => r.Name).ToList();

                        var choices = new List<Choice>();
                        resources.ForEach(r => choices.Add(new Choice { Value = r }));
                        choices.Add(new Choice { Value = Phrases.None });

                        return await dialogContext.PromptAsync(
                            Prompt.ResourcePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Request.Resources(userContext.Category),
                                Choices = choices,
                                Validations = new ResourcePromptValidations { Category = userContext.Category }
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Choice was validated in case the schema changed.
                        var selectedResource = ((FoundChoice)dialogContext.Result).Value;

                        if (selectedResource == Phrases.None)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        // Store the resource in the user context.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.Resource = selectedResource;

                        // Ask the distance they want to broadcast to.
                        return await dialogContext.PromptAsync(
                                Prompt.IntPrompt,
                                new PromptOptions
                                {
                                    Prompt = Phrases.Request.Distance
                                },
                                cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var searchDistance = (int)dialogContext.Result;

                        var user = await api.GetUser(dialogContext.Context);
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        // This section is resource intensive. Should look into a way to do geofenced search.
                        var resources = await this.api.GetResources(userContext.Category, userContext.Resource);
                        List<UserResourcePair> matches = new List<UserResourcePair>();

                        foreach (var resource in resources)
                        {
                            // TODO: Make unit of length customizable. Maybe set in helpers and have it used throughout (also in text response).
                            Coordinates userCoordinates = new Coordinates(user.LocationLatitude, user.LocationLongitude);
                            Coordinates resourceCoordinates = new Coordinates(resource.User.LocationLatitude, resource.User.LocationLongitude);
                            var distance = userCoordinates.DistanceTo(resourceCoordinates, UnitOfLength.Miles);

                            // TODO: Ask for range they want to request from.
                            if (distance < searchDistance)
                            {
                                matches.Add(resource);
                            }
                        }


                        // TODO: add to outgoing message queue.


                        await Messages.SendAsync(Phrases.Request.Sent(matches.Count), turnContext, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
