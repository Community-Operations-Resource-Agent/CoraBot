using Microsoft.Azure.Cosmos.Spatial;

namespace Shared.Models
{
    public class User : Model
    {
        public bool IsConsentGiven { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsGreyshirt { get; set; }

        public string Language { get; set; }

        public string Location { get; set; }

        public Point LocationCoordinates { get; set; }

        public bool ContactEnabled { get; set; }

        public User() : base()
        {
            this.ContactEnabled = true;
        }
    }
}
