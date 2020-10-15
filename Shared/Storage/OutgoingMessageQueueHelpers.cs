using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Shared.Storage
{
    public class OutgoingMessageQueueHelpers : QueueHelpers
    {
        public const string QueueName = "outgoing-messages";

        private readonly string connectionString;

        public OutgoingMessageQueueHelpers(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Enqueue(OutgoingMessageQueueData data)
        {
            await AddMessage(connectionString, QueueName, JsonConvert.SerializeObject(data));
        }
    }

    public class OutgoingMessageQueueData
    {
        public string PhoneNumber { get; set; }

        public string Message { get; set; }
    }
}
