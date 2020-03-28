using EntityModel.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class User : Model
    {
        public static string TABLE_NAME = "";

        [JsonIgnore]
        [JsonProperty(PropertyName = "")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "")]
        public double LocationLatitude { get; set; }

        [JsonProperty(PropertyName = "")]
        public double LocationLongitude { get; set; }

        [JsonProperty(PropertyName = "")]
        public DayFlags ReminderFrequency { get; set; }

        [JsonProperty(PropertyName = "")]
        public string ReminderTime { get; set; }

        [JsonProperty(PropertyName = "")]
        public bool ContactEnabled { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

        public class Resolver : ContractResolver<User>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "");
            }
        }
    }
}
