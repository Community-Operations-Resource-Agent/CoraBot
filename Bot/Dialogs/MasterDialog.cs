using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using System.Threading;
using System.Threading.Tasks;
using Shared;
using System.Linq;
using Shared.Prompts;
using System;
using EntityModel;
using Bot.State;

namespace Bot.Dialogs
{
    public class MasterDialog : DialogBase
    {
        public static string Name = typeof(MasterDialog).FullName;

        public MasterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Get the user.
                        var user = await api.GetUser(dialogContext.Context);
                       
                        if (user == null)
                        {
                            // Create a new user.
                            user = new User { PhoneNumber = Helpers.GetUserToken(turnContext) };
                            await this.api.Create(user);

                            await Messages.SendAsync(Phrases.Greeting.WelcomeNew, turnContext, cancellationToken);
                        }

                        // Check if the initial message is one of the keywords.
                        var incomingMessage = dialogContext.Context.Activity.Text;
                        if (!string.IsNullOrEmpty(incomingMessage))
                        {
                            bool isKeyword = Phrases.Keywords.List.Any(k => string.Equals(incomingMessage, k, StringComparison.OrdinalIgnoreCase));
                            if (isKeyword)
                            {
                                return await dialogContext.NextAsync(incomingMessage);
                            }
                        }

                        // Prompt for a keyword.
                        return await dialogContext.PromptAsync(
                            Prompt.GreetingTextPrompt,
                            new PromptOptions {
                                Prompt = Phrases.Greeting.GetKeywords(),
                                RetryPrompt = Phrases.Greeting.GetKeywords()
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = dialogContext.Result as string;

                        if (string.Equals(result, Phrases.Keywords.Provide, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the provide dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, ProvideDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Request, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the request dialog onto the stack.
                            return await BeginDialogAsync(dialogContext, RequestDialog.Name, null, cancellationToken);
                        }
                        else if (string.Equals(result, Phrases.Keywords.Options, StringComparison.OrdinalIgnoreCase))
                        {
                            // Push the options dialog onto the stack.
                            //return await BeginDialogAsync(dialogContext, OptionsDialog.Name, null, cancellationToken);
                        }

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
