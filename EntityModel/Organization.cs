using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityModel
{
    public class Organization : Model
    {
        public static string TABLE_NAME = "";

        [JsonProperty(PropertyName = "")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "")]
        public bool IsVerified { get; set; }

        [JsonProperty(PropertyName = "")]
        public string PhoneNumber { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

        public class Resolver : ContractResolver<Organization>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "");
            }
        }
    }
}
