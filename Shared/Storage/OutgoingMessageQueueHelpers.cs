using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Shared.Storage
{
    public class OutgoingMessageQueueHelpers : QueueHelpers
    {
        public const string QueueName = "outgoing-messages";

        private string connectionString;

        public OutgoingMessageQueueHelpers(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Enqueue(OutgoingMessageQueueData data)
        {
            await AddMessage(this.connectionString, QueueName, JsonConvert.SerializeObject(data));
        }
    }

    public class OutgoingMessageQueueData
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
