namespace Web.Integrations.Dtos;

public class GolfNowFacilitiesRequest
{
    public uint Radius { get; init; } = 25;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public int PageSize { get; } = 30;
    public int PageNumber { get; } = 0;
    public int SearchType { get; } = 0;
    public string SortBy { get; } = "Facilities.Distance";
    public string SortDirection { get; } = "0";
    public string Date { get; init; }
    public string View { get; } = "Course";
}