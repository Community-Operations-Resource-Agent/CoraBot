namespace Shared
{
    public enum Units
    {
        Miles,
        Kilometers
    }

    public static class UnitExtensions
    {
        const double KilometersPerMile = 1.609344;
        const double MetersPerKilometer = 1000;

        public static double ToMeters(this Units units, int value)
        {
            switch (units)
            {
                case Units.Miles: return value * KilometersPerMile * MetersPerKilometer;
                case Units.Kilometers: return value * MetersPerKilometer;
                default: return 0;
            }
        }
    }
}
