using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public class Feedback : Model
    {
        public static string TABLE_NAME = "";

        [JsonIgnore]
        [JsonProperty(PropertyName = "")]
        public string SenderId { get; set; }

        [JsonProperty(PropertyName = "")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "")]
        public string Text { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

        public Feedback() : base()
        {
            this.CreatedOn = DateTime.UtcNow;
        }

        public class Resolver : ContractResolver<Feedback>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "");
            }
        }
    }
}
