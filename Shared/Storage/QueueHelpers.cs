using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System.Threading.Tasks;

namespace Shared.Storage
{
    public class QueueHelpers
    {
        public async Task AddMessage(string connectionString, string queueName, string message)
        {
            var queue = await Init(connectionString, queueName);
            await queue.AddMessageAsync(new CloudQueueMessage(message));
        }

        public async Task<CloudQueueMessage> GetMessage(string connectionString, string queueName)
        {
            var queue = await Init(connectionString, queueName);
            return await queue.GetMessageAsync();
        }

        public async Task DeleteMessage(string connectionString, string queueName, CloudQueueMessage message)
        {
            var queue = await Init(connectionString, queueName);
            await queue.DeleteMessageAsync(message);
        }

        private async Task<CloudQueue> Init(string connectionString, string queueName)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            await queue.CreateIfNotExistsAsync();

            return queue;
        }
    }
}
