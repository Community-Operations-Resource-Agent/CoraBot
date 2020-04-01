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
        public LocationAddress Address { get; set; }

        public LocationPosition Position { get; set; }
    }

    public class LocationAddress
    {
        public string Municipality { get; set; }
        public string CountrySecondarySubdivision { get; set; }
        public string countrySubdivision { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return $"{this.Municipality}, {this.CountrySecondarySubdivision}, {this.countrySubdivision}, {this.Country}";
        }
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
