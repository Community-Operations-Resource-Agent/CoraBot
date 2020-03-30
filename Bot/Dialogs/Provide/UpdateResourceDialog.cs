using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
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
                        var resource = await this.api.GetResourceForUser(user, userContext.Category, userContext.Resource);

                        // Prompt for the quantity.
                        return await dialogContext.PromptAsync(
                            Prompt.IntPrompt,
                            new PromptOptions
                            {
                                Prompt = Phrases.Provide.GetQuantity(resource.Name)
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);
                        var userContext = await this.state.GetUserContext(dialogContext.Context, cancellationToken);
                        var resource = await this.api.GetResourceForUser(user, userContext.Category, userContext.Resource);

                        var quantity = (int)dialogContext.Result;
                        if (quantity == 0)
                        {
                            await this.api.Delete(resource);
                        }
                        else
                        {
                            resource.Quantity = quantity;
                            resource.CreatedOn = DateTime.UtcNow;
                            await this.api.Update(resource);
                        }

                        await Messages.SendAsync(Phrases.Provide.CompleteUpdate, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
