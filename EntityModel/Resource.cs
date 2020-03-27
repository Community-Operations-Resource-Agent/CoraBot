using EntityModel.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public abstract class Resource : Model
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

        /// <summary>
        /// The primary key for referencing the type in the data store.
        /// </summary>
        public abstract string PrimaryKey();

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
