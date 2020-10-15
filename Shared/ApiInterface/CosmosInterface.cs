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
        private readonly IConfiguration config;

        private readonly CosmosClient client;

        private readonly Database database;

        public CosmosInterface(IConfiguration config)
        {
            this.config = config;
            client = new CosmosClient(config.CosmosEndpoint(), config.CosmosKey());
            database = client.GetDatabase(this.config.CosmosDatabase());
        }

        /// <summary>
        /// Does any initialization required for the data store.
        /// </summary>
        public async Task Init()
        {
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(config.CosmosDatabase());
            var database = databaseResponse.Database;

            await database.CreateContainerIfNotExistsAsync(config.CosmosConversationsContainer(), "/id");
            await database.CreateContainerIfNotExistsAsync(config.CosmosUsersContainer(), "/PhoneNumber");
            await database.CreateContainerIfNotExistsAsync(config.CosmosNeedsContainer(), "/Category");
            await database.CreateContainerIfNotExistsAsync(config.CosmosResourcesContainer(), "/Category");
            await database.CreateContainerIfNotExistsAsync(config.CosmosFeedbackContainer(), "/id");
        }

        /// <summary>
        /// Removes all data from the data store. Make sure you REALLY want to do this!
        /// </summary>
        public async Task Destroy()
        {
            var database = client.GetDatabase(config.CosmosDatabase());
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
        public async Task<Models.User> GetUser(ITurnContext turnContext)
        {
            var userToken = Helpers.GetUserToken(turnContext);
            return await GetUser(userToken);
        }

        /// <summary>
        /// Gets a user from a phone number.
        /// </summary>
        public async Task<Models.User> GetUser(string phoneNumber)
        {
            var container = database.GetContainer(config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Models.User>()
                .Where(u => u.PhoneNumber == phoneNumber)
                .ToFeedIterator();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public async Task<List<Models.User>> GetUsers()
        {
            var container = database.GetContainer(config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Models.User>()
                .ToFeedIterator();

            var result = new List<Models.User>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                result.AddRange(response.Resource);
            }

            return result;
        }

        /// <summary>
        /// Gets all user within a distance from coordinates.
        /// </summary>
        public async Task<List<Models.User>> GetUsersWithinDistance(Point coordinates, double distanceMeters)
        {
            var container = database.GetContainer(config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Models.User>()
                .Where(u => u.LocationCoordinates.Distance(coordinates) <= distanceMeters)
                .ToFeedIterator();

            var result = new List<Models.User>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                result.AddRange(response.Resource);
            }

            return result;
        }

        /// <summary>
        /// Gets all user within a distance from coordinates that also match the provided phone numbers.
        /// </summary>
        public async Task<List<Models.User>> GetUsersWithinDistance(Point coordinates, double distanceMeters, List<string> phoneNumbers)
        {
            var container = database.GetContainer(config.CosmosUsersContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Models.User>()
                .Where(u => u.LocationCoordinates.Distance(coordinates) <= distanceMeters && phoneNumbers.Contains(u.PhoneNumber))
                .ToFeedIterator();

            var result = new List<Models.User>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                result.AddRange(response.Resource);
            }

            return result;
        }

        /// <summary>
        /// Gets a resource for a user.
        /// </summary>
        public async Task<Resource> GetResourceForUser(Models.User user, string category, string resource)
        {
            var container = database.GetContainer(config.CosmosResourcesContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Resource>()
                .Where(r => r.CreatedById == user.Id && r.Category == category && r.Name == resource)
                .ToFeedIterator();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets a need for a user.
        /// </summary>
        public async Task<Need> GetNeedForUser(Models.User user, string category, string resource)
        {
            var container = database.GetContainer(config.CosmosNeedsContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Need>()
                .Where(n => n.CreatedById == user.Id && n.Category == category && n.Name == resource)
                .ToFeedIterator();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        /// <summary>
        /// Gets a need from an ID.
        /// </summary>
        public async Task<Need> GetNeedById(string id)
        {
            var container = database.GetContainer(config.CosmosNeedsContainer());
            if (container == null)
            {
                return null;
            }

            var queryIterator = container.GetItemLinqQueryable<Need>()
                .Where(n => n.Id == id)
                .ToFeedIterator();

            var response = await queryIterator.ReadNextAsync();
            return response.Resource.FirstOrDefault();
        }

        private Container GetContainer(Model model)
        {
            if (model is Models.User)
            {
                return database.GetContainer(config.CosmosUsersContainer());
            }
            else if (model is Resource)
            {
                return database.GetContainer(config.CosmosResourcesContainer());
            }
            else if (model is Need)
            {
                return database.GetContainer(config.CosmosNeedsContainer());
            }
            else if (model is Feedback)
            {
                return database.GetContainer(config.CosmosFeedbackContainer());
            }

            return null;
        }

        private string GetPartitionKey(Model model)
        {
            if (model is Models.User)
            {
                return ((Models.User)model).PhoneNumber;
            }
            else if (model is Resource)
            {
                return ((Resource)model).Category;
            }
            else if (model is Need)
            {
                return ((Need)model).Category;
            }

            return model.Id;
        }
    }
}
