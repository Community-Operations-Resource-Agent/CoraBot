using Bot.State;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Provide
{
    public class CreateResourceDialog : DialogBase
    {
        public static string Name = typeof(CreateResourceDialog).FullName;

        public CreateResourceDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
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
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);

                        var resource = new Resource();
                        resource.CreatedById = user.Id;
                        resource.Category = userContext.Category.Name;
                        resource.Name = userContext.Resource.Name;
                        resource.HasQuantity = userContext.Resource.HasQuantity;
                        await this.api.Create(resource);

                        if (resource.HasQuantity)
                        {
                            // Prompt for the quantity.
                            return await dialogContext.PromptAsync(
                                Prompt.IntPrompt,
                                new PromptOptions
                                {
                                    Prompt = Phrases.Provide.GetQuantity
                                },
                                cancellationToken);
                        }

                        // Skip this step.
                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        var resource = await this.api.GetResourceForUser(user, userContext.Category.Name, userContext.Resource.Name);

                        // Check if the previous step had a result.
                        if (dialogContext.Result != null)
                        {
                            resource.Quantity = (int)dialogContext.Result;
                        }

                        resource.IsRecordComplete = true;
                        await this.api.Update(resource);

                        await Messages.SendAsync(Phrases.Provide.CompleteCreate(user), dialogContext.Context, cancellationToken);

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
