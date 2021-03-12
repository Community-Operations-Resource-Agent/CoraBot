using Remy.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using Shared.Translation;
using System.Threading;
using System.Threading.Tasks;

namespace Remy.Dialogs.Need
{
    public class FoodAssistanceDialog : DialogBase
    {
        public static string Name = typeof(FoodAssistanceDialog).FullName;

        Translator translator;

        public FoodAssistanceDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
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
                        // TODO: Cosmos lookup for programs near user
                        var user = await this.api.GetUserFromContext(turnContext);

                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("FoodAssistanceExample", new { userLocation = user.Location }, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    },
                });
            });
        }
    }
}
