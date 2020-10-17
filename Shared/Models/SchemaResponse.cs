using System.Collections.Generic;

namespace Shared.Models
{
    public class SchemaResponse
    {
        public List<SchemaVerifiedOrganization> VerifiedOrganizations { get; set; }

        public List<SchemaCategory> Categories { get; set; }

        public SchemaResponse()
        {
            VerifiedOrganizations = new List<SchemaVerifiedOrganization>();
        }
    }

    public class SchemaVerifiedOrganization
    {
        public string Name { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public SchemaVerifiedOrganization()
        {
            PhoneNumbers = new List<string>();
        }
    }

    public class SchemaCategory
    {
        public string Name { get; set; }

        public List<SchemaResource> Resources { get; set; }

        public SchemaCategory()
        {
            Resources = new List<SchemaResource>();
        }
    }

    public class SchemaResource
    {
        public string Name { get; set; }
    }
}
