using Bot.State;
using Microsoft.Azure.Documents.Spatial;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs.Preferences
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
                                Prompt = Phrases.Preferences.GetLocation,
                                RetryPrompt = Phrases.Preferences.GetLocationRetry
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        if (dialogContext.Result != null)
                        {
                            var location = (string)dialogContext.Result;
                            var position = await Helpers.LocationToPosition(configuration, location);

                            // Save the location. It was validated by the prompt.
                            var user = await this.api.GetUser(dialogContext.Context);
                            user.Location = location;
                            user.LocationCoordinates = new Point(position.Lon, position.Lat);
                            await this.api.Update(user);
                        }

                        await Messages.SendAsync(Phrases.Preferences.LocationUpdated, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
