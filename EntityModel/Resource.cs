using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public class Resource : Model
    {
        public static string TABLE_NAME = "";

        [JsonProperty(PropertyName = "")]
        public string OrganizationId { get; set; }

        [JsonProperty(PropertyName = "")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "")]
        public int Max { get; set; }

        [JsonProperty(PropertyName = "")]
        public int Available { get; set; }

        [JsonProperty(PropertyName = "")]
        public bool HasWaitlist { get; set; }

        [JsonProperty(PropertyName = "")]
        public bool IsWaitlistOpen { get; set; }

        [JsonProperty(PropertyName = "")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "")]
        public string CreatedById { get; set; }

        [JsonProperty(PropertyName = "")]
        public bool IsRecordComplete { get; set; }

        public override IContractResolver ContractResolver() { return Resolver.Instance; }
        public override string TableName() { return TABLE_NAME; }

        public Resource() : base()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.Name = this.CreatedOn.ToString("yyyy/MM/dd hh:mm tt");
        }

        public class Resolver : ContractResolver<Resource>
        {
            public static Resolver Instance = new Resolver();

            private Resolver()
            {
                AddMap(x => x.Id, "");
            }
        }
    }
}
