using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs.Missions
{
    public class AcceptMissionDialog : DialogBase
    {
        public static string Name = typeof(AcceptMissionDialog).FullName;

        public AcceptMissionDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base(state, dialogs, api, configuration, lgGenerator) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Format is "Accept XXXXX".
                        string[] tokens = dialogContext.Context.Activity.Text.Split(" ");
                        if (tokens.Length != 2)
                        {
                            await Messages.SendAsync(Phrases.Need.InvalidFormat, dialogContext.Context, cancellationToken);
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        var mission = await this.api.GetMissionByShortId(tokens[1]);
                        if (mission == null)
                        {
                            await Messages.SendAsync(Phrases.Need.InvalidId, dialogContext.Context, cancellationToken);
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        if (!string.IsNullOrEmpty(mission.AssignedToId))
                        {
                            await Messages.SendAsync(Phrases.Need.AlreadyAssigned, dialogContext.Context, cancellationToken);
                            return await dialogContext.NextAsync(null, cancellationToken);
                        }

                        var missionCreater = await this.api.GetUserFromId(mission.CreatedById);
                        await Messages.SendAsync(Phrases.Match.Accepted(missionCreater.PhoneNumber), dialogContext.Context, cancellationToken);
                        return await dialogContext.NextAsync(null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
