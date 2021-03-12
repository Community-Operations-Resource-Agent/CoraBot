using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Remy.State;
using Shared.ApiInterface;
using Shared.Prompts;
using Shared.Translation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Remy.Dialogs.Need
{
    public class ShoppingDeliveryDialog : DialogBase
    {
        public static string Name = typeof(ShoppingDeliveryDialog).FullName;

        Translator translator;

        public ShoppingDeliveryDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
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
                        // TODO: Get from LG
                        var choices = new List<Choice>();
                        Phrases.ShoppingDelivery.WhenOptions.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("GetShoppingDeliveryTime", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        // TODO save information

                        // TODO: Get from LG
                        var choices = new List<Choice>();
                        Phrases.ShoppingDelivery.MethodOptions.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = ActivityFactory.FromObject(this.lgGenerator.Generate("GetShoppingDeliveryMethod", null, turnContext.Activity.Locale)),
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        // TODO save information

                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("ShoppingDeliveryConfirmation", null, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    },
                });
            });
        }
    }
}
