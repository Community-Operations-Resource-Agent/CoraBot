using EntityModel;
using Microsoft.Bot.Builder;
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
        /// Gets a resource for a user.
        /// </summary>
        Task<Resource> GetResourceForUser(User user, string category, string resource);
    }
}
