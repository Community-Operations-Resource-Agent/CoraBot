using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Remy.State;
using Shared.ApiInterface;
using Shared.Prompts;
using Shared.Translation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remy.Dialogs.Need
{
    public class FoodBankDialog : DialogBase
    {
        public static string Name = typeof(FoodBankDialog).FullName;

        Translator translator;

        public FoodBankDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator)
        {
            this.translator = new Translator(configuration);
        }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // TODO: Cosmos lookup for food banks near user
                        var user = await this.api.GetUserFromContext(turnContext);

                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("FoodBanksExample", new { userLocation = user.Location }, turnContext.Activity.Locale)));
                        
                        // TODO: Get from LG
                        var choices = new List<Choice>{new Choice("Yes"), new Choice("No") };
                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("DoesThisHelp", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        if (string.Equals(result, "No", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = new List<Choice>{new Choice("Yes"), new Choice("No") };
                            return await dialogContext.PromptAsync(
                                Prompt.ChoicePrompt,
                                new PromptOptions()
                                {
                                    Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("NeedAssistanceQuestion", null, turnContext.Activity.Locale)),
                                    Choices = choices
                                },
                                cancellationToken);
                        }
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        if (string.Equals(result, "Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            return await BeginDialogAsync(dialogContext, ShoppingDeliveryDialog.Name, null, cancellationToken);
                        }
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
