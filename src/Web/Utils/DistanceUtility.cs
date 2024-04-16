using Web.Dtos;

namespace Web.Utils;

public static class DistanceUtility
{
    /// <summary>
    /// Calculate the distance between two coordinates in miles
    /// </summary>
    /// <param name="co1"></param>
    /// <param name="co2"></param>
    /// <returns></returns>
    /// <see href="https://stackoverflow.com/a/21623206/5508690"/>
    public static double Haversine(Coordinates co1, Coordinates co2)
    {
        const double earthRadiusMiles = 3958; // miles
        const double piOver180 = Math.PI / 180;

        var lat1 = co1.Latitude * piOver180;
        var lon1 = co1.Longitude * piOver180;
        var lat2 = co2.Latitude * piOver180;
        var lon2 = co2.Longitude * piOver180;

        var a = 0.5 - Math.Cos((lat2 - lat1) / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) * (1 - Math.Cos((lon2 - lon1) / 2));

        return 2 * earthRadiusMiles * Math.Asin(Math.Sqrt(a));
    }
}