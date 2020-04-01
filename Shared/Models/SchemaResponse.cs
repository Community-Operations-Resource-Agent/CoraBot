using System.Collections.Generic;

namespace Shared.Models
{
    public class SchemaResponse
    {
        public SchemaUnits Units { get; set; }
        public List<SchemaVerifiedOrganization> VerifiedOrganizations { get; set; }
        public List<SchemaCategory> Categories { get; set; }

        public SchemaResponse()
        {
            this.VerifiedOrganizations = new List<SchemaVerifiedOrganization>();
        }
    }

    public class SchemaVerifiedOrganization
    {
        public string Name { get; set; }
        public List<string> PhoneNumbers { get; set; }

        public SchemaVerifiedOrganization()
        {
            this.PhoneNumbers = new List<string>();
        }
    }

    public class SchemaCategory
    {
        public string Name { get; set; }
        public List<SchemaResource> Resources { get; set; }

        public SchemaCategory()
        {
            this.Resources = new List<SchemaResource>();
        }
    }

    public class SchemaResource
    {
        public string Name { get; set; }
    }

    public enum SchemaUnits
    {
        Miles,
        Kilometers
    }

    public static class SchemaUnitsExtensions
    {
        const double KilometersPerMile = 1.609344;
        const double MetersPerKilometer = 1000;

        public static double ToMeters(this SchemaUnits units, int value)
        {
            switch (units)
            {
                case SchemaUnits.Miles: return value * KilometersPerMile * MetersPerKilometer;
                case SchemaUnits.Kilometers: return value * MetersPerKilometer;
                default: return 0;
            }
        }
    }
}
