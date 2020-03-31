using Microsoft.Azure.Documents.Spatial;
using Shared.Models.Helpers;

namespace Shared.Models
{
    public class User : Model
    {
        public string PhoneNumber { get; set; }

        public string Location { get; set; }

        public Point LocationCoordinates { get; set; }

        public DayFlags ReminderFrequency { get; set; }

        public string ReminderTime { get; set; }

        public bool ContactEnabled { get; set; }

        public User() : base()
        {
            this.ReminderFrequency = DayFlags.Everyday;
            this.ContactEnabled = true;
        }
    }
}
