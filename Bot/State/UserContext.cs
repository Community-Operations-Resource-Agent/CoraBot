using Shared.Models;

namespace Bot.State
{
    public class UserContext
    {
        public int TimezoneOffset { get; set; }
        public string Category { get; set; }
        public string Resource { get; set; }
        public int RequestQuantity { get; set; }
        public int RequestDistance { get; set; }
    }
}