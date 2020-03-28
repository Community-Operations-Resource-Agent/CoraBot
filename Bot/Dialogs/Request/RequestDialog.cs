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
                        var user = await api.GetUser(dialogContext.Context);

                        if (string.IsNullOrEmpty(user.Location))
                        {
                            // Push the update location dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                        }

                        // Skip this step.
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
                            Prompt.CategoryPrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Request.Categories,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var schema = Helpers.GetSchema();

                        // Category was validated so it is guaranteed to be in the schema.
                        var selectedCategory = ((FoundChoice)dialogContext.Result).Value;

                        // Store the category in the user context.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.Category = schema.Categories.FirstOrDefault(c => c.Name == selectedCategory);

                        // Get the resources in the category.
                        var category = schema.Categories.FirstOrDefault(c => c.Name == selectedCategory);
                        List<string> resources = category.Resources.Select(r => r.Name).ToList();

                        var choices = new List<Choice>();
                        resources.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ResourcePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Request.Resources(selectedCategory),
                                Choices = choices,
                                Validations = new ResourcePromptValidations { Category = selectedCategory }
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Resource was validated so it is guaranteed to be in the schema.
                        var selectedResource = ((FoundChoice)dialogContext.Result).Value;

                        // Store the resource in the user context.
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.Resource = userContext.Category.Resources.FirstOrDefault(r => r.Name == selectedResource);

                         return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        // This section is resource intensive. Should look into a way to do geofenced search.
                        var resources = await this.api.GetResources(userContext.Category.Name, userContext.Resource.Name);
                        UserResourcePair closestMatch = null;
                        double closestMatchDistance = double.MaxValue;

                        foreach (var resource in resources)
                        {
                            // TODO: Make unit of length customizable. Maybe set in helpers and have it used throughout (also in text response).
                            Coordinates userCoordinates = new Coordinates(user.LocationLatitude, user.LocationLongitude);
                            Coordinates resourceCoordinates = new Coordinates(resource.User.LocationLatitude, resource.User.LocationLongitude);
                            var distance = userCoordinates.DistanceTo(resourceCoordinates, UnitOfLength.Miles);

                            if (closestMatch == null || 
                                distance < closestMatchDistance ||
                                (distance == closestMatchDistance && resource.Resource.Quantity > closestMatch.Resource.Quantity))
                            {
                                closestMatch = resource;
                                closestMatchDistance = distance;

                                // Optimization? No need to search the whole world if there is something nearby.
                                /*
                                if (distance < 25)
                                {
                                    break;
                                }
                                */
                            }
                        }

                        await Messages.SendAsync(Phrases.Request.Match(closestMatch, closestMatchDistance), turnContext, cancellationToken);

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
