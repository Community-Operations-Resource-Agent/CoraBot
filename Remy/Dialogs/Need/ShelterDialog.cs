using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Remy.State;
using Shared.ApiInterface;
using Shared.Translation;
using System.Threading;
using System.Threading.Tasks;

namespace Remy.Dialogs.Need
{
    public class ShelterDialog : DialogBase
    {
        public static string Name = typeof(ShelterDialog).FullName;

        Translator translator;

        public ShelterDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
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
                        // TODO: Cosmos lookup for shelters near user
                        var user = await this.api.GetUserFromContext(turnContext);

                        await turnContext.SendActivityAsync(ActivityFactory.FromObject(this.lgGenerator.Generate("SheltersExample", new { userLocation = user.Location }, turnContext.Activity.Locale)));
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    },
                });
            });
        }
    }
}
