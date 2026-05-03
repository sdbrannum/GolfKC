namespace Web.Integrations.Dtos;

public class GolfNowTeeTimesRequest
{
    public uint Radius { get; } = 30;
    public double Latitude { get; } = 39.01470395078284;
    public double Longitude { get; } = -94.69324882561136;
    public int PageSize { get; } = 30;
    public int PageNumber { get; } = 0;
    public string SearchType { get; } = "Facility";
    public string SortBy { get; } = "Date";
    public int SortDirection { get; } = 0;
    public string Date { get; set; } = default!;
    public string Players { get; set; } = "0";
    public string Holes { get; set; } = "18";
    public string FacilityId { get; set; } = default!;
    public string SortByRollup { get; } = "Date.MinDate";
    public string View { get; } = "Grouping";
}