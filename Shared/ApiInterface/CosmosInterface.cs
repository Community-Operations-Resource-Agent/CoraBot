using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ApiInterface
{
    /// <summary>
    /// API interface for Cosmos
    /// </summary>
    public class CosmosInterface : IApiInterface
    {
        private IConfiguration config;
        private CosmosClient client;
        private Database database;

        public CosmosInterface(IConfiguration config)
        {
            this.config = config;
            this.client = new CosmosClient(config.CosmosEndpoint(), config.CosmosKey());
            this.database = this.client.GetDatabase(this.config.CosmosDatabase());
        }

        /// <summary>
        /// Does any initialization required for the data store.
        /// </summary>
        public async Task Init()
        {
            var databaseResponse = await this.client.CreateDatabaseIfNotExistsAsync(this.config.CosmosDatabase());
            var database = databaseResponse.Database;

            await database.CreateContainerIfNotExistsAsync(this.config.CosmosConversationsContainer(), "/id");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosUsersContainer(), "/PhoneNumber");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosMissionsContainer(), "/id");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosFeedbackContainer(), "/id");
        }

        /// <summary>
        /// Removes all data from the data store. Make sure you REALLY want to do this!
        /// </summary>
        public async Task Destroy()
        {
            var database = this.client.GetDatabase(this.config.CosmosDatabase());
            await database.DeleteAsync();
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        public async Task<string> Create<T>(T model) where T : Model
        {
            var container = GetContainer(model);
            if (container == null)
            {
                return string.Empty;
            }

            var response = await container.CreateItemAsync(model, new PartitionKey(GetPartitionKey(model)));
            return response.Resource.Id;
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        public async Task<bool> Delete<T>(T model) where T : Model
        {
            var container = GetContainer(model);
            if (container == null)
            {
                return false;
            }

            await container.DeleteItemAsync<Model>(model.Id, new PartitionKey(GetPartitionKey(model)));
            return true;
        }

        /// <summary>
        /// Saves changes to a record.
        /// </summary>
        public async Task<bool> Update<T>(T model) where T : Model
        {
            var container = GetContainer(model);
            if (container == null)
            {
                return false;
            }

            await container.ReplaceItemAsync(model, model.Id, new PartitionKey(GetPartitionKey(model)));
            return true;
        }

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        public async Task<Models.User> GetUserFromContext(ITurnContext turnContext)
        {
            var userToken = Helpers.GetUserToken(turnContext);
            return await GetUserFromPhoneNumber(userToken);
        }

        /// <summary>
        /// Gets a Greyshirt from a turn context.
        /// </summary>
        public async Task<Greyshirt> GetGreyshirtFromContext(ITurnContext turnContext)
        {
            var userToken = Helpers.GetUserToken(turnContext);
            return await GetGreyshirtFromPhoneNumber(userToken);
        }

        /// <summary>
        /// Gets a user from a phone number.
        /// </summary>
        public async Task<Models.User> GetUserFromPhoneNumber(string phoneNumber)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var query = container.GetItemLinqQueryable<Models.User>()
                .Where(u => u.PhoneNumber == phoneNumber);

            var queryIterator = query.ToFeedIterator();
            var result = new List<Models.User>();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets a Greyshirt from a phone number.
        /// </summary>
        public async Task<Greyshirt> GetGreyshirtFromPhoneNumber(string phoneNumber)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var query = container.GetItemLinqQueryable<Greyshirt>()
                .Where(u => u.IsGreyshirt && u.PhoneNumber == phoneNumber);

            var queryIterator = query.ToFeedIterator();
            var result = new List<Greyshirt>();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets a user from an ID.
        /// </summary>
        public async Task<Models.User> GetUserFromId(string id)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var query = container.GetItemLinqQueryable<Models.User>()
                .Where(u => u.Id == id);

            var queryIterator = query.ToFeedIterator();
            var result = new List<Models.User>();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets a Greyshirt from an ID.
        /// </summary>
        public async Task<Greyshirt> GetGreyshirtFromId(string id)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var query = container.GetItemLinqQueryable<Greyshirt>()
                .Where(u => u.IsGreyshirt && u.Id == id);

            var queryIterator = query.ToFeedIterator();
            var result = new List<Greyshirt>();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets all users within a distance from coordinates.
        /// </summary>
        public async Task<List<Models.User>> GetUsersWithinDistance(Point coordinates, double distanceMeters)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var query = container.GetItemLinqQueryable<Models.User>()
                .Where(u => u.LocationCoordinates.Distance(coordinates) <= distanceMeters);

            var queryIterator = query.ToFeedIterator();
            var result = new List<Models.User>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                result.AddRange(response.Resource);
            }

            return result;
        }

        /// <summary>
        /// Gets all Greyshirts within a distance from coordinates.
        /// </summary>
        public async Task<List<Greyshirt>> GetGreyshirtsWithinDistance(Point coordinates, double distanceMeters)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var query = container.GetItemLinqQueryable<Greyshirt>()
                .Where(u => u.IsGreyshirt && u.LocationCoordinates.Distance(coordinates) <= distanceMeters);

            var queryIterator = query.ToFeedIterator();
            var result = new List<Greyshirt>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                result.AddRange(response.Resource);
            }

            return result;
        }

        /// <summary>
        /// Gets all missions for a user.
        /// </summary>
        public async Task<List<Mission>> GetMissionsForUser(Models.User user, bool createdByUser, bool isAssigned)
        {
            var container = this.database.GetContainer(this.config.CosmosMissionsContainer());
            if (container == null)
            {
                return null;
            }

            var orderedQueryable = container.GetItemLinqQueryable<Mission>();

            var queryable = createdByUser ?
                orderedQueryable.Where(m => m.CreatedById == user.Id) :
                orderedQueryable.Where(m => m.AssignedToId == user.Id);

            queryable = isAssigned ?
                queryable.Where(m => !m.AssignedToId.IsNull()) :
                queryable.Where(m => m.AssignedToId.IsNull());

            var feedIterator = queryable.ToFeedIterator();

            var result = new List<Mission>();

            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();
                result.AddRange(response.Resource);
            }

            return result;
        }

        /// <summary>
        /// Gets a need from an ID.
        /// </summary>
        public async Task<Mission> GetMissionById(string id)
        {
            var container = this.database.GetContainer(this.config.CosmosMissionsContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Mission>()
                .Where(m => m.Id == id)
                .ToFeedIterator();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets a mission from a short ID.
        /// </summary>
        public async Task<Mission> GetMissionByShortId(string id)
        {
            var container = this.database.GetContainer(this.config.CosmosMissionsContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Mission>()
                .Where(m => m.ShortId == id)
                .ToFeedIterator();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        private Container GetContainer(Model model)
        {
            if (model is Models.User)
            {
                return this.database.GetContainer(this.config.CosmosUsersContainer());
            }
            else if (model is Mission)
            {
                return this.database.GetContainer(this.config.CosmosMissionsContainer());
            }
            else if (model is Feedback)
            {
                return this.database.GetContainer(this.config.CosmosFeedbackContainer());
            }

            return null;
        }

        private string GetPartitionKey(Model model)
        {
            if (model is Models.User)
            {
                return ((Models.User)model).PhoneNumber;
            }

            return model.Id;
        }
    }
}
