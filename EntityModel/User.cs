using EntityModel.Helpers;

namespace EntityModel
{
    public class User : Model
    {
        public string PhoneNumber { get; set; }

        public string Location { get; set; }

        public double LocationLatitude { get; set; }

        public double LocationLongitude { get; set; }

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
