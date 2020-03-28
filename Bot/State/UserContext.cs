using Shared.Models;

namespace Bot.State
{
    public class UserContext
    {
        public int TimezoneOffset { get; set; }
        public ResourceSchemaCategory Category { get; set; }
        public ResourceSchemaResource Resource { get; set; }
    }
}