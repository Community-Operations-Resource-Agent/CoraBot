using EntityModel;
using EntityModel.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.EntityFrameworkCore;
using System;
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
        /// Creates a new record.
        /// </summary>
        public async Task<string> Create(Model model)
        {
            if (model is User)
            {
                await this.dbContext.Users.AddAsync(model as User);
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
        /// Deletes a record.
        /// </summary>
        public async Task<bool> Delete(Model model)
        {
            if (model is User)
            {
                this.dbContext.Users.Remove(model as User);
            }
            else if (model is Resource)
            {
                this.dbContext.Resources.Remove(model as Resource);
            }
            else if (model is Feedback)
            {
                this.dbContext.Feedback.Remove(model as Feedback);
            }
            else
            {
                Debug.Assert(false, "Add the new type");
                return false;
            }

            await this.dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Saves changes to a record.
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

        /// <summary>
        /// Checks if a user has any resources.
        /// </summary>
        public async Task<bool> UserHasResources(User user)
        {
            return await this.dbContext.Resources.AnyAsync(r => r.CreatedById == user.Id);
        }

        /// <summary>
        /// Gets all resource for a user.
        /// </summary>
        public async Task<List<Resource>> GetResourcesForUser(User user)
        {
            return await this.dbContext.Resources.Where(r => r.CreatedById == user.Id).ToListAsync();
        }

        /// <summary>
        /// Gets a resource for a user.
        /// </summary>
        public async Task<Resource> GetResourceForUser(User user, string category, string resource)
        {
            return await this.dbContext.Resources.FirstOrDefaultAsync(r => r.CreatedById == user.Id && r.Category == category && r.Name == resource);
        }

        /// <summary>
        /// Gets all resources of a given type.
        /// </summary>
        public async Task<List<UserResourcePair>> GetResources(string category, string resource)
        {
            return await this.dbContext.Users.Join(this.dbContext.Resources,
                u => u.Id,
                r => r.CreatedById,
                (u, r) => new UserResourcePair { User = u, Resource = r })
                .Where(p => p.Resource.Category == category && p.Resource.Name == resource)
                .ToListAsync();
        }
    }    
}
