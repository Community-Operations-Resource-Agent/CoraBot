using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
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
            // Make sure the database and all collections exist.
            var databaseResponse = await this.client.CreateDatabaseIfNotExistsAsync(this.config.CosmosDatabase());
            var database = databaseResponse.Database;

            await database.CreateContainerIfNotExistsAsync(this.config.CosmosConversationsContainer(), "/id");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosUsersContainer(), "/PhoneNumber");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosNeedsContainer(), "/Category");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosResourcesContainer(), "/Category");
            await database.CreateContainerIfNotExistsAsync(this.config.CosmosFeedbackContainer(), "/id");
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

            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator:
                case Channels.Webchat:
                case Channels.Sms:
                {
                    return await GetUser(userToken);
                }
                default: return null;
            }
        }

        /// <summary>
        /// Gets a user from a phone number.
        /// </summary>
        public async Task<Models.User> GetUser(string phoneNumber)
        {
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
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
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
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
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
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
            var container = this.database.GetContainer(this.config.CosmosUsersContainer());
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
            var container = this.database.GetContainer(this.config.CosmosResourcesContainer());
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
            var container = this.database.GetContainer(this.config.CosmosNeedsContainer());
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
            var container = this.database.GetContainer(this.config.CosmosNeedsContainer());
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
                return this.database.GetContainer(this.config.CosmosUsersContainer());
            }
            else if (model is Resource)
            {
                return this.database.GetContainer(this.config.CosmosResourcesContainer());
            }
            else if (model is Need)
            {
                return this.database.GetContainer(this.config.CosmosNeedsContainer());
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

        /*
        private FeedOptions GetPartitionedFeedOptions()
        {
            // From https://docs.microsoft.com/en-us/azure/cosmos-db/performance-tips

            // If you don't know the number of partitions, you can set the degree of
            // parallelism to a high number. The system will choose the minimum
            // (number of partitions, user provided input) as the degree of parallelism.

            // When maxItemCount is set to -1, the SDK automatically finds the optimal
            // value, depending on the document size
            return new FeedOptions
            {
                EnableCrossPartitionQuery = true,
                MaxDegreeOfParallelism = 100,
                MaxItemCount = -1,
            };
        }
        */
    }
}
