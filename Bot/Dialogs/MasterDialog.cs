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
using Bot.Dialogs.Request;
using Bot.Dialogs.Provide;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;

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
                                if (string.Equals(incomingMessage, Phrases.Keywords.Update, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Push the provide dialog onto the stack.
                                    return await BeginDialogAsync(dialogContext, ProvideDialog.Name, null, cancellationToken);
                                }
                            }
                        }

                        // Prompt for an option.
                        var choices = new List<Choice>();
                        Phrases.Greeting.GetOptionsList().ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Phrases.Greeting.GetOptions,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result is FoundChoice choice)
                        {
                            var result = choice.Value;

                            if (string.Equals(result, Phrases.Options.Request, StringComparison.OrdinalIgnoreCase))
                            {
                                // Push the request dialog onto the stack.
                                return await BeginDialogAsync(dialogContext, RequestDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.Provide, StringComparison.OrdinalIgnoreCase))
                            {
                                // Push the provide dialog onto the stack.
                                return await BeginDialogAsync(dialogContext, ProvideDialog.Name, null, cancellationToken);
                            }
                            else if (string.Equals(result, Phrases.Options.MoreOptions, StringComparison.OrdinalIgnoreCase))
                            {
                                // Push the options dialog onto the stack.
                                return await BeginDialogAsync(dialogContext, OptionsExtendedDialog.Name, null, cancellationToken);

                            }
                        }

                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Clear the user context when a new converation begins.
                        await this.state.ClearUserContext(dialogContext.Context, cancellationToken);

                        return await dialogContext.ReplaceDialogAsync(MasterDialog.Name, null, cancellationToken);
                    }
                });
            });
        }
    }
}
