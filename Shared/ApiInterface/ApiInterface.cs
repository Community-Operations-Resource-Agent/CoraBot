using EntityModel;
using Microsoft.Bot.Builder;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    public interface IApiInterface
    {
        /// <summary>
        /// Creates a new record of a model.
        /// </summary>
        Task<string> Create(Model model);

        /// <summary>
        /// Saves changes to a model.
        /// </summary>
        Task<bool> Update(Model model);

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        Task<User> GetUser(ITurnContext turnContext);
    }
}
