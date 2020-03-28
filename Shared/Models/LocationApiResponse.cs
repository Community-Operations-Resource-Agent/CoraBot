using System.Collections.Generic;

namespace Shared.Models
{
    public class LocationApiResponse
    {
        public LocationSummary Summary { get; set; }
        public List<LocationResult> Results { get; set; }
    }

    public class LocationSummary
    {
        public int NumResults { get; set; }
    }

    public class LocationResult
    {
        public EntityType EntityType { get; set; }

        public LocationPosition Position { get; set; }
    }

    public class LocationPosition
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public enum EntityType
    {
        Country,
        CountrySecondarySubdivision,
        CountrySubdivision,
        CountryTertiarySubdivision,
        Municipality,
        MunicipalitySubdivision,
        Neighbourhood,
        PostalCodeArea
    }
}
