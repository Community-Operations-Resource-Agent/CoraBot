using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System;
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
                        resource.Category = userContext.Category;
                        resource.Name = userContext.Resource;
                        await this.api.Create(resource);

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

                            await Messages.SendAsync(Phrases.Provide.CompleteUpdate, dialogContext.Context, cancellationToken);
                        }
                        else
                        {
                            resource.Quantity = quantity;
                            resource.CreatedOn = DateTime.UtcNow;
                            resource.IsRecordComplete = true;
                            await this.api.Update(resource);

                            await Messages.SendAsync(Phrases.Provide.CompleteCreate(user), dialogContext.Context, cancellationToken);
                        }

                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
