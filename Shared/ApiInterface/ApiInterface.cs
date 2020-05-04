using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Bot.Builder;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    public interface IApiInterface
    {
        /// <summary>
        /// Does any initialization required for the data store.
        /// </summary>
        Task Init();

        /// <summary>
        /// Removes all data from the data store. Make sure you REALLY want to do this!
        /// </summary>
        Task Destroy();

        /// <summary>
        /// Creates a new record.
        /// </summary>
        Task<string> Create<T>(T model) where T : Model;

        /// <summary>
        /// Deletes a record.
        /// </summary>
        Task<bool> Delete<T>(T model) where T : Model;

        /// <summary>
        /// Saves changes to a record.
        /// </summary>
        Task<bool> Update<T>(T model) where T : Model;

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        Task<User> GetUser(ITurnContext turnContext);

        /// <summary>
        /// Gets a Greyshirt from a turn context.
        /// </summary>
        Task<Greyshirt> GetGreyshirt(ITurnContext turnContext);

        /// <summary>
        /// Gets a user from a phone number.
        /// </summary>
        Task<User> GetUser(string phoneNumber);

        /// <summary>
        /// Gets a Greyshirt from a phone number.
        /// </summary>
        Task<Greyshirt> GetGreyshirt(string phoneNumber);

        /// <summary>
        /// Gets all users within a distance from coordinates.
        /// </summary>
        Task<List<User>> GetUsersWithinDistance(Point coordinates, double distanceMeters);

        /// <summary>
        /// Gets all Greyshirts within a distance from coordinates.
        /// </summary>
        Task<List<Greyshirt>> GetGreyshirtsWithinDistance(Point coordinates, double distanceMeters);

        /// <summary>
        /// Gets all missions for a user.
        /// </summary>
        Task<List<Mission>> GetMissionsForUser(User user, bool createdByUser, bool isAssigned);

        /// <summary>
        /// Gets a mission from an ID.
        /// </summary>
        Task<Mission> GetMissionById(string id);
    }
}
