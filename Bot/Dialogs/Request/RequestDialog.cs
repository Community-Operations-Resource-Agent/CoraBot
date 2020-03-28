using Bot.Dialogs.Preferences;
using Bot.State;
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
                        userContext.Cateogry = schema.Categories.FirstOrDefault(c => c.Name == selectedCategory);

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
                        userContext.Resource = userContext.Cateogry.Resources.FirstOrDefault(r => r.Name == selectedResource);

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
