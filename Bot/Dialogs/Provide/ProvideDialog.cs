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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Provide
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
                        // Get the categories.
                        var schema = Helpers.GetSchema();
                        var categories = schema.Categories.Select(c => c.Name).ToList();

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
                                Prompt = Phrases.Provide.GetResource(userContext.Category),
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

                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.Resource = selectedResource;

                        // Prompt for the quantity.
                        return await dialogContext.PromptAsync(
                            Prompt.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Provide.GetQuantity(selectedResource) },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.ProvideQuantity = (int)dialogContext.Result;

                        if (userContext.ProvideQuantity == 0)
                        {
                            // Delete the resource if it has already been added.
                            var user = await api.GetUser(dialogContext.Context);
                            var resource = await this.api.GetResourceForUser(user, userContext.Category, userContext.Resource);
                            if (resource != null)
                            {
                                await this.api.Delete(resource);
                                await Messages.SendAsync(Phrases.Provide.CompleteDelete, dialogContext.Context, cancellationToken);
                            }
                            else
                            {
                                await Messages.SendAsync(Phrases.Provide.CompleteUpdate, dialogContext.Context, cancellationToken);
                            }

                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        // Prompt for whether or not it is unopened.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Provide.GetIsUnopened },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        // Check if they have already added this resource.
                        var user = await api.GetUser(dialogContext.Context);
                        var resource = await this.api.GetResourceForUser(user, userContext.Category, userContext.Resource);

                        // Create or update the resource.
                        if (resource == null)
                        {
                            resource = new Resource();
                            resource.CreatedById = user.Id;
                            resource.Category = userContext.Category;
                            resource.Name = userContext.Resource;
                            resource.Quantity = userContext.ProvideQuantity;
                            resource.IsUnopened = (bool)dialogContext.Result;
                            await this.api.Create(resource);

                            await Messages.SendAsync(Phrases.Provide.CompleteCreate(user), dialogContext.Context, cancellationToken);
                        }
                        else
                        {
                            resource.CreatedOn = DateTime.UtcNow;
                            resource.Quantity = userContext.ProvideQuantity;
                            resource.IsUnopened = (bool)dialogContext.Result;
                            await this.api.Update(resource);

                            await Messages.SendAsync(Phrases.Provide.CompleteUpdate, dialogContext.Context, cancellationToken);
                        }

                        // Check for any existing needs that match this resource.
                        var schema = Helpers.GetSchema();
                        var verifiedOrganizationPhoneNumbers = schema.VerifiedOrganizations.SelectMany(o => o.PhoneNumbers).ToList();
                        double requestMeters = Units.Miles.ToMeters(50);

                        // Get all verified organizations within the distance from the user.
                        var orgUsersWithinDistance = await this.api.GetUsersWithinDistance(user.LocationCoordinates, requestMeters, verifiedOrganizationPhoneNumbers);
                        if (orgUsersWithinDistance.Count > 0)
                        {
                            // Get any matching needs.
                            foreach (var orgUser in orgUsersWithinDistance)
                            {
                                var need = await this.api.GetNeedForUser(orgUser, resource.Category, resource.Name);

                                if (!Helpers.DoesResourceMatchNeed(need, resource))
                                {
                                    continue;
                                }

                                userContext.ProvideMatches.Add(new Match { OrgPhoneNumber = orgUser.PhoneNumber, NeedId = need.Id });
                            }

                            // If there were matches, present them to the user.
                            if (userContext.ProvideMatches.Count > 0)
                            {
                                return await BeginDialogAsync(dialogContext, MatchDialog.Name, null, cancellationToken);
                            }
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Check if they want to register another resource.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Provide.Another },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if ((bool)dialogContext.Result)
                        {
                            return await dialogContext.ReplaceDialogAsync(ProvideDialog.Name, null, cancellationToken);
                        }

                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
