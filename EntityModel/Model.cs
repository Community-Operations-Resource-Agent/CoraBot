using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public abstract class Model
    {
        [Key]
        [JsonIgnore]
        public string Id { get; set; }

        public abstract IContractResolver ContractResolver();
        public abstract string TableName();

        public Model()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
