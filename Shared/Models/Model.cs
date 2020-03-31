using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public abstract class Model
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Model()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
