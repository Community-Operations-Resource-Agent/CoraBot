using Bot.State;
using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Provide
{
    public class UpdateResourceDialog : DialogBase
    {
        public static string Name = typeof(UpdateResourceDialog).FullName;

        public UpdateResourceDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
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
                        var resource = await this.api.GetResourceForUser(user, userContext.Category.Name, userContext.Resource.Name);

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
                        else
                        {
                            // Ask if the resourece is still available.
                            return await dialogContext.PromptAsync(
                                Prompt.ConfirmPrompt,
                                new PromptOptions { Prompt = Phrases.Provide.GetAvailable },
                                cancellationToken);
                        }
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        var resource = await this.api.GetResourceForUser(user, userContext.Category.Name, userContext.Resource.Name);

                        // Check the result of the previous step.
                        if ((dialogContext.Result is int quantity && quantity == 0) ||
                            (dialogContext.Result is bool isAvailable && !isAvailable))
                        {
                            await this.api.Delete(resource);
                        }
                        else if (dialogContext.Result is int q)
                        {
                            resource.Quantity = q;
                            await this.api.Update(resource);
                        }

                        await Messages.SendAsync(Phrases.Provide.CompleteUpdate, dialogContext.Context, cancellationToken);

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
