using System.Collections.Generic;

namespace Shared.Models
{
    public class ResourceDefinitionsResponse
    {
        public List<ResourceDefinitionsCategory> Categories { get; set; }
    }

    public class ResourceDefinitionsCategory
    {
        public string Name { get; set; }
        public List<ResourceDefinitionsResource> Resources { get; set; }
    }

    public class ResourceDefinitionsResource
    {
        public string Name { get; set; }
    }
}
