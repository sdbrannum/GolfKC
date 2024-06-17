using System.Text.Json.Serialization;

namespace Web.Integrations.Dtos;

public class GolfBackTeeTimesResponse
{
    [JsonPropertyName("data")]
    public IEnumerable<GolfBackTeeTime> TeeTimes { get; set; }
}

public class GolfBackTeeTime
{
    public string Id { get; set; } = default!;
    public string CourseId { get; set; } = default!;
    public string CourseName { get; set; } = default!;
    public string DateTime { get; set; } = default!;
    public DateTime LocalDateTime { get; set; }
    public IEnumerable<GolfBackRate> Rates { get; set; }
    public bool IsAvailable { get; set; }
    public IEnumerable<int> Holes { get; set; }
    public bool Has9Holes { get; set; }
    public bool HasDeal { get; set; }
    public IEnumerable<PrimaryPrices> PrimaryPrices { get; set; }
    public int PlayersMin { get; set; }
    public int PlayersMax { get; set; }
    public string PlayersDisplay { get; set; } = default!;
}

public class GolfBackRate
{
    public string RatePlanId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Holes { get; set; }
    public bool HasCartIncluded { get; set; }
    public bool IsPrimary { get; set; }
    public bool UsePrimaryAfterSelection { get; set; }
    public bool IsDeal { get; set; }
    public bool IsGimme { get; set; }
    public double BasePrice { get; set; }
    public double Price { get; set; }
}

public class PrimaryPrices
{
    public int Holes { get; set; }
    public double? BasePrice { get; set; }
    public double? Price { get; set; }
}

