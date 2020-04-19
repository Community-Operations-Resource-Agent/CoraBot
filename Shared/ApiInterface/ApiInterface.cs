using Microsoft.Azure.Documents.Spatial;
using Microsoft.Bot.Builder;
using Shared.Models;
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
        /// Gets a user from a phone number.
        /// </summary>
        Task<User> GetUser(string phoneNumber);

        /// <summary>
        /// Gets all users.
        /// </summary>
        Task<List<User>> GetUsers();

        /// <summary>
        /// Gets all user within a distance from coordinates.
        /// </summary>
        Task<List<User>> GetUsersWithinDistance(Point coordinates, double distanceMeters);

        /// <summary>
        /// Gets all user within a distance from coordinates that also match the provided phone numbers.
        /// </summary>
        Task<List<User>> GetUsersWithinDistance(Point coordinates, double distanceMeters, List<string> phoneNumbers);

        /// <summary>
        /// Gets a resource for a user.
        /// </summary>
        Task<Resource> GetResourceForUser(User user, string category, string resource);

        /// <summary>
        /// Gets a need for a user.
        /// </summary>
        Task<Need> GetNeedForUser(User user, string category, string resource);

        /// <summary>
        /// Gets a need from an ID.
        /// </summary>
        Task<Need> GetNeedById(string id);
    }
}
