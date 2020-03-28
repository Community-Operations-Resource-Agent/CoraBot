using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public abstract class Model
    {
        [Key]
        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Model()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
