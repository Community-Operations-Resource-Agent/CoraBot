using EntityModel;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    /// <summary>
    /// API interface for Entity Framework
    /// </summary>
    public class EfInterface : IApiInterface
    {
        private DbModel dbContext { get; }

        public EfInterface(DbModel dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new record of a model.
        /// </summary>
        public async Task<string> Create(Model model)
        {
            if (model is User)
            {
                await this.dbContext.Users.AddAsync(model as User);
            }
            else if (model is Organization)
            {
                await this.dbContext.Organizations.AddAsync(model as Organization);
            }
            else if (model is Resource)
            {
                await this.dbContext.Resources.AddAsync(model as Resource);
            }
            else if (model is Feedback)
            {
                await this.dbContext.Feedback.AddAsync(model as Feedback);
            }
            else
            {
                Debug.Assert(false, "Add the new type");
                return string.Empty;
            }

            await this.dbContext.SaveChangesAsync();
            return model.Id;
        }

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        public async Task<bool> Update(Model model)
        {
            await this.dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        public async Task<User> GetUser(ITurnContext turnContext)
        {
            var userToken = Helpers.GetUserToken(turnContext);

            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator:
                case Channels.Sms: return await this.dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userToken);
                default: Debug.Fail("Missing channel type"); return null;
            }
        }
    }    
}
