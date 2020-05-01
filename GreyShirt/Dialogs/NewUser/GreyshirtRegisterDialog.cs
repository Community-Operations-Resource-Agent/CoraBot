using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
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

        public GreyshirtRegisterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

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
                            new PromptOptions { Prompt = Phrases.Register.GetIsRegistered },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (!(bool)dialogContext.Result)
                        {
                            await Messages.SendAsync(Phrases.Register.HowToRegister, turnContext, cancellationToken);

                            return await dialogContext.PromptAsync(
                                Prompt.IntPrompt,
                                new PromptOptions {
                                    Prompt = Phrases.Register.GetNumberNew,
                                    RetryPrompt = Phrases.Register.GetNumberNewRepeat
                                },
                                cancellationToken);
                        }
                        else
                        {
                            return await dialogContext.PromptAsync(
                                Prompt.IntPrompt,
                                new PromptOptions {  Prompt = Phrases.Register.GetNumberExisting },
                                cancellationToken);
                        }
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await this.api.GetUser(turnContext);
                        user.GreyshirtNumber = (int)dialogContext.Result;
                        await this.api.Update(user);

                        await Messages.SendAsync(Phrases.Register.GetNumberConfirm, turnContext, cancellationToken);
                        return await dialogContext.EndDialogAsync(true, cancellationToken);
                    },
                });
            });
        }
    }
}
