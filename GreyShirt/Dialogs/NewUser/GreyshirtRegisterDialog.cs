using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs.NewUser
{
    public class GreyshirtRegisterDialog : DialogBase
    {
        public static string Name = typeof(GreyshirtRegisterDialog).FullName;

        public GreyshirtRegisterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("GetIsRegistered", null, turnContext.Activity.Locale)) },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (!(bool)dialogContext.Result)
                        {
                            await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("HowToRegister", null, turnContext.Activity.Locale)));
                            await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("ComeBackLater", null, turnContext.Activity.Locale)));

                            // Mark them as not greyshirt
                            var greyshirt = await api.GetGreyshirtFromContext(dialogContext.Context);
                            greyshirt.IsGreyshirt = false;
                            await this.api.Update(greyshirt);

                            return await dialogContext.EndDialogAsync(false, cancellationToken);
                        }
                        else
                        {
                            return await dialogContext.PromptAsync(
                                Prompt.IntPrompt,
                                new PromptOptions {  Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("GetNumberExisting", null, turnContext.Activity.Locale)) },
                                cancellationToken);
                        }
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var greyshirt = await this.api.GetGreyshirtFromContext(turnContext);
                        greyshirt.GreyshirtNumber = (int)dialogContext.Result;
                        greyshirt.IsGreyshirt = true;
                        await this.api.Update(greyshirt);

                        await dialogContext.Context.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("GetNumberConfirm", null, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(true, cancellationToken);
                    },
                });
            });
        }
    }
}
