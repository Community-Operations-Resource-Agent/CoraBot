using EntityModel;
using EntityModel.Helpers;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    public interface IApiInterface
    {
        /// <summary>
        /// Creates a new record.
        /// </summary>
        Task<string> Create(Model model);

        /// <summary>
        /// Deletes a record.
        /// </summary>
        Task<bool> Delete(Model model);

        /// <summary>
        /// Saves changes to a record.
        /// </summary>
        Task<bool> Update(Model model);

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        Task<User> GetUser(ITurnContext turnContext);

        /// <summary>
        /// Checks if a user has any resources.
        /// </summary>
        Task<bool> UserHasResources(User user);

        /// <summary>
        /// Gets all resource for a user.
        /// </summary>
        Task<List<Resource>> GetResourcesForUser(User user);

        /// <summary>
        /// Gets a resource for a user.
        /// </summary>
        Task<Resource> GetResourceForUser(User user, string category, string resource);

        /// <summary>
        /// Gets all resources of a given type.
        /// </summary>
        Task<List<UserResourcePair>> GetResources(string category, string resource);
    }
}
