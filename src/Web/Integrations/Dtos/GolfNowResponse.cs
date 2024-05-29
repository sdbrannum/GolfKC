using System.Text.Json.Serialization;

namespace Web.Integrations.Dtos;

public class GolfNowResponse
{
    [JsonPropertyName("ttResults")]
    public GolfNowResponseData Data { get; init; }
}

public class GolfNowResponseData
{
    public IEnumerable<GolfNowFacility>? Facilities { get; init; } = Enumerable.Empty<GolfNowFacility>();
    public IEnumerable<GolfNowTeeTime>? TeeTimes { get; init; }
}

public class GolfNowTeeTime
{
    public GolfNowFacility Facility { get; init; } = default!;

    /// <summary>
    /// aka holes
    /// </summary>
    public string MultipleHolesRate { get; init; } = default!;
    public int PlayerRule { get; init; }
    public DateTime Time { get; init; }
    public double MinTeeTimeRate { get; init; }
    public double MaxTeeTimeRate { get; init; }
    public double DisplayRate { get; init; }
    public string FormattedTime { get; init; } = default!;
    public string FormattedTimeMeridian { get; init; } = default!;
}

public class GolfNowFacility
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public GolfNowFacilityAddress Address { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double? Distance { get; init; }
    public DateTime MinDate { get; init; }
    /// <summary>
    /// The first time available
    /// </summary>
    public string MinDateFormatted { get; init; } = default!;
    public DateTime MaxDate { get; init; }
    /// <summary>
    /// The last time available
    /// </summary>
    public string MaxDateFormatted { get; init; } = default!;
    public double MinPrice { get; init; }
    public double MaxPrice { get; init; }
    public string ThumbnailImagePath { get; init; } = default!;
    public int NumberOfTeeTimes { get; init; }
    public int GolfRange { get; init; }
}

public class GolfNowFacilityAddress
{
    public string Line1 { get; init; } = default!;
    public string? Line2 { get; init; }
    public string City { get; init; } = default!;
    public string StateProvinceCode { get; init; } = default!;
    public string PostalCode { get; init; } = default!;
    public string Country { get; init; } = default!;
    public string StateProvince { get; init; } = default!;
}