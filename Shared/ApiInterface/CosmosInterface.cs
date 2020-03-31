using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Shared.Models;
using Shared.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private DocumentClient client;

        public CosmosInterface(IConfiguration config)
        {
            this.config = config;
            this.client = new DocumentClient(new Uri(config.CosmosEndpoint()), config.CosmosKey());
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        public async Task<string> Create(Model model)
        {
            var collection = GetCollection(model);
            if (string.IsNullOrEmpty(collection))
            {
                return string.Empty;
            }

            var collectionUri = UriFactory.CreateDocumentCollectionUri(this.config.CosmosDatabase(), collection);
            await client.CreateDocumentAsync(collectionUri, model);
            return model.Id;
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        public async Task<bool> Delete(Model model)
        {
            var collection = GetCollection(model);
            if (string.IsNullOrEmpty(collection))
            {
                return false;
            }

            var documentUri = UriFactory.CreateDocumentUri(this.config.CosmosDatabase(), collection, model.Id);
            await client.DeleteDocumentAsync(documentUri);
            return true;
        }

        /// <summary>
        /// Saves changes to a record.
        /// </summary>
        public async Task<bool> Update(Model model)
        {
            var collection = GetCollection(model);
            if (string.IsNullOrEmpty(collection))
            {
                return false;
            }

            var documentUri = UriFactory.CreateDocumentUri(this.config.CosmosDatabase(), collection, model.Id);
            await client.ReplaceDocumentAsync(documentUri, model);
            return true;
        }

        /// <summary>
        /// Gets a user from a turn context.
        /// </summary>
        public Task<User> GetUser(ITurnContext turnContext)
        {
            User user = null;
            var userToken = Helpers.GetUserToken(turnContext);

            switch (turnContext.Activity.ChannelId)
            {
                case Channels.Emulator:
                case Channels.Webchat:
                case Channels.Sms:
                    {
                        user = this.client.CreateDocumentQuery<User>(
                            UriFactory.CreateDocumentCollectionUri(
                                this.config.CosmosDatabase(),
                                this.config.CosmosUsersCollection()))
                            .Where(u => u.PhoneNumber == userToken)
                            .AsEnumerable()
                            .FirstOrDefault();
                    }
                    break;
                default: Debug.Fail("Missing channel type"); break;
            }

            return Task.FromResult(user);
        }

        /// <summary>
        /// Gets all user within a distance from coordinates.
        /// </summary>
        public Task<List<User>> GetUsersWithinDistance(Point coordinates, double distanceMeters)
        {
            // Get all users within the distance.
            var result = this.client.CreateDocumentQuery<User>(
                UriFactory.CreateDocumentCollectionUri(
                    this.config.CosmosDatabase(),
                    this.config.CosmosUsersCollection()))
                .Where(u => u.LocationCoordinates.Distance(coordinates) <= 30000)
                .ToList();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Checks if a user has any resources.
        /// </summary>
        public Task<bool> UserHasResources(User user)
        {
            var result = this.client.CreateDocumentQuery<Resource>(
                UriFactory.CreateDocumentCollectionUri(
                    this.config.CosmosDatabase(),
                    this.config.CosmosResourcesCollection()), 
                 new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(r => r.CreatedById == user.Id)
                .Take(1)
                .AsEnumerable()
                .Any();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Gets all resource for a user.
        /// </summary>
        public Task<List<Resource>> GetResourcesForUser(User user)
        {
            var result = this.client.CreateDocumentQuery<Resource>(
                UriFactory.CreateDocumentCollectionUri(
                    this.config.CosmosDatabase(),
                    this.config.CosmosResourcesCollection()),
                new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(r => r.CreatedById == user.Id)
                .ToList();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Gets a resource for a user.
        /// </summary>
        public Task<Resource> GetResourceForUser(User user, string category, string resource)
        {
            var result = this.client.CreateDocumentQuery<Resource>(
                UriFactory.CreateDocumentCollectionUri(
                    this.config.CosmosDatabase(),
                    this.config.CosmosResourcesCollection()),
                new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(r => r.CreatedById == user.Id && r.Category == category && r.Name == resource)
                .AsEnumerable()
                .FirstOrDefault();

            return Task.FromResult(result);
        }

        private string GetCollection(Model model)
        {
            if (model is User)
            {
                return this.config.CosmosUsersCollection();
            }
            else if (model is Resource)
            {
                return this.config.CosmosResourcesCollection();
            }
            else if (model is Feedback)
            {
                return this.config.CosmosFeedbackCollection();
            }
            else
            {
                Debug.Assert(false, "Add the new type");
                return string.Empty;
            }
        }
    }
}
