using System.Collections.Generic;

namespace Shared.Models
{
    public class ResourceSchemaResponse
    {
        public List<ResourceSchemaCategory> Categories { get; set; }
    }

    public class ResourceSchemaCategory
    {
        public string Name { get; set; }
        public List<ResourceSchemaResource> Resources { get; set; }
    }

    public class ResourceSchemaResource
    {
        public string Name { get; set; }
        public bool HasQuantity { get; set; }

        public ResourceSchemaResource()
        {
            this.HasQuantity = true;
        }
    }
}
