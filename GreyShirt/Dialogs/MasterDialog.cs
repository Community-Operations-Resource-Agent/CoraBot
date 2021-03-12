using Greyshirt.Dialogs.NewUser;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Clear the user context when a new converation begins.
                        await this.state.ClearUserContext(dialogContext.Context, cancellationToken);

                        // Handle any keywords.
                        if (Phrases.Keywords.IsKeyword(dialogContext.Context.Activity.Text))
                        {
                            var greyshirt = await api.GetGreyshirtFromContext(dialogContext.Context);
                            if (greyshirt.IsConsentGiven && greyshirt.IsRegistered())
                            {
                                return await BeginDialogAsync(dialogContext, KeywordDialog.Name, null, cancellationToken);
                            }
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // The keyword flow can result in ending the conversation.
                        if (dialogContext.Result is bool continueConversation && !continueConversation)
                        {
                            await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("Goodbye", null, turnContext.Activity.Locale)));
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Register the user if they are new.
                        var greyshirt = await api.GetGreyshirtFromContext(dialogContext.Context);
                        if (greyshirt == null)
                        {
                            greyshirt = new Shared.Models.Greyshirt();
                        }
                        if (!greyshirt.IsConsentGiven || !greyshirt.IsGreyshirt)
                        {
                            return await BeginDialogAsync(dialogContext, NewUserDialog.Name, null, cancellationToken);
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // The new user flow can result in no consent or not registered. If so, end the conversation.
                        if (!(bool)dialogContext.Result)
                        {
                            return await dialogContext.EndDialogAsync(null, cancellationToken);
                        }

                        // Start the main menu flow.
                        return await BeginDialogAsync(dialogContext, MenuDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("Goodbye", null, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
