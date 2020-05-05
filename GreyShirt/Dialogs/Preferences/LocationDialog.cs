using Greyshirt.State;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs.Preferences
{
    public class LocationDialog : DialogBase
    {
        public static string Name = typeof(LocationDialog).FullName;

        public LocationDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Prompt for the location.
                        return await dialogContext.PromptAsync(
                            Prompt.LocationTextPrompt,
                            new PromptOptions
                            {
                                Prompt = Shared.Phrases.Preferences.GetLocation,
                                RetryPrompt = Shared.Phrases.Preferences.GetLocationRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        // Location was validated.
                        var locationString = (string)dialogContext.Result;
                        var location = await Helpers.StringToLocation(configuration, locationString);

                        var greyshirt = await this.api.GetGreyshirtFromContext(dialogContext.Context);
                        greyshirt.Location = location.Address.ToCityStateString();
                        greyshirt.LocationCoordinates = new Point(location.Position.Lon, location.Position.Lat);
                        await this.api.Update(greyshirt);

                        // Check if the location is correct.
                        return await dialogContext.PromptAsync(
                            Prompt.ConfirmPrompt,
                            new PromptOptions { Prompt = Shared.Phrases.Preferences.GetLocationConfirm(location.Address.ToCityStateString()) },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (!(bool)dialogContext.Result)
                        {
                            return await dialogContext.ReplaceDialogAsync(LocationDialog.Name, null, cancellationToken);
                        }

                        await Messages.SendAsync(Shared.Phrases.Preferences.LocationUpdated, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
