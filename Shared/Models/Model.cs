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
            Id = Guid.NewGuid().ToString();
            CreatedOn = DateTime.UtcNow;
        }
    }
}
