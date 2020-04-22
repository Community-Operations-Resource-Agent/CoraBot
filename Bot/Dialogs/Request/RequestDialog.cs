using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using Shared.Storage;
using Shared.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Request
{
    public class RequestDialog : DialogBase
    {
        public static string Name = typeof(RequestDialog).FullName;

        Translator translator;

        public RequestDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration)
        {
            this.translator = new Translator(configuration);
        }

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

                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.Resource = selectedResource;

                        // Ask how many they need.
                        return await dialogContext.PromptAsync(
                            Prompt.IntPrompt,
                            new PromptOptions { Prompt = Phrases.Request.GetQuantity(selectedResource) },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.NeedQuantity = (int)dialogContext.Result;

                        if (userContext.NeedQuantity == 0)
                        {
                            // Delete the need if it has already been added.
                            var user = await api.GetUser(dialogContext.Context);
                            var need = await this.api.GetNeedForUser(user, userContext.Category, userContext.Resource);
                            if (need != null)
                            {
                                await this.api.Delete(need);
                                await Messages.SendAsync(Phrases.Request.CompleteDelete, dialogContext.Context, cancellationToken);
                            }
                            else
                            {
                                await Messages.SendAsync(Phrases.Request.CompleteUpdate, dialogContext.Context, cancellationToken);
                            }

                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        // Ask whether or not they are willing to take opened items.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Phrases.Request.GetOpenedOkay },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        userContext.NeedUnopenedOnly = !(bool)dialogContext.Result;

                        // Ask for any instructions.
                        return await dialogContext.PromptAsync(
                            Prompt.TextPrompt,
                            new PromptOptions { Prompt = Phrases.Request.Instructions },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var schema = Helpers.GetSchema();
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        // Check if they have already added this resource.
                        var user = await api.GetUser(dialogContext.Context);
                        var need = await this.api.GetNeedForUser(user, userContext.Category, userContext.Resource);

                        // Create or update the need.
                        if (need == null)
                        {
                            need = new Need();
                            need.CreatedById = user.Id;
                            need.Category = userContext.Category;
                            need.Name = userContext.Resource;
                            need.Quantity = userContext.NeedQuantity;
                            need.UnopenedOnly = userContext.NeedUnopenedOnly;
                            need.Instructions = (string)dialogContext.Result;
                            await this.api.Create(need);
                        }
                        else
                        {
                            need.CreatedOn = DateTime.UtcNow;
                            need.Quantity = userContext.NeedQuantity;
                            need.UnopenedOnly = userContext.NeedUnopenedOnly;
                            need.Instructions = (string)dialogContext.Result;
                            await this.api.Update(need);
                        }

                        // The API input requires meters.
                        // Default is 50, but we could make this configurable in the future.
                        double requestMeters = Units.Miles.ToMeters(50);

                        // Get all users within the distance from the user.
                        var usersWithinDistance = await this.api.GetUsersWithinDistance(user.LocationCoordinates, requestMeters);
                        if (usersWithinDistance.Count > 0)
                        {
                            var organization = schema.VerifiedOrganizations.FirstOrDefault(o => o.PhoneNumbers.Contains(user.PhoneNumber));
                            var message = Phrases.Match.GetMessage(organization.Name, need.Name, need.Quantity, need.Instructions);
                            var queueHelper = new OutgoingMessageQueueHelpers(this.configuration.AzureWebJobsStorage());

                            // Cache any translations to limit API calls.
                            var translationCache = new Dictionary<string, string>();

                            // Get any matching resources for the users.
                            foreach (var userWithinDistance in usersWithinDistance)
                            {
                                var resource = await this.api.GetResourceForUser(userWithinDistance, need.Category, need.Name);

                                if (!Helpers.DoesResourceMatchNeed(need, resource))
                                {
                                    continue;
                                }

                                // Check if the user's language is already cached.
                                if (translationCache.TryGetValue(user.Language, out var translation))
                                {
                                    message = translation;
                                }
                                else
                                {
                                    // Translate the message if necessary.
                                    if (translator.IsConfigured && user.Language != Translator.DefaultLanguage)
                                    {
                                        message = await translator.TranslateAsync(message, user.Language);
                                    }

                                    // Cache the message
                                    translationCache.Add(user.Language, message);
                                }

                                var data = new OutgoingMessageQueueData
                                {
                                    PhoneNumber = userWithinDistance.PhoneNumber,
                                    Message = message
                                };

                                await queueHelper.Enqueue(data);
                            }
                        }

                        await Messages.SendAsync(Phrases.Request.CompleteCreate(user), turnContext, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
