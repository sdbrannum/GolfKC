namespace Web.Integrations.Dtos;

public class GolfNowTeeTimesRequest
{
    public uint Radius { get; init; } = 25;
    public double Latitude { get; init; }
    public double Longitude { get; set; }
    public int PageSize { get; } = 30;
    public int PageNumber { get; } = 0;
    public int SearchType { get; } = 1;
    public string SortBy { get; } = "Date";
    public int SortDirection { get; } = 0;
    public string Date { get; set; } = default!;
    public string Players { get; set; } = "0";
    public string Holes { get; set; } = "18";
    public string FacilityId { get; set; } = default!;
    public string SortByRollup { get; } = "Date.MinDate";
    public string View { get; } = "Grouping";
}