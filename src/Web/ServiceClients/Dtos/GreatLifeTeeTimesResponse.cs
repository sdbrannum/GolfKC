using System.Text.Json.Serialization;

namespace Web.ServiceClients.Dtos;

public class GreatLifeTeeTimesResponse
{
    [JsonPropertyName("data")]
    public IEnumerable<GreatLifeTeeTime> TeeTimes { get; set; }
}

public class GreatLifeTeeTime
{
    public string Id { get; set; }
    public string CourseId { get; set; }
    public string CourseName { get; set; }
    public string DateTime { get; set; }
    public DateTime LocalDateTime { get; set; }
    public IEnumerable<GreatLifeRate> Rates { get; set; }
    public bool IsAvailable { get; set; }
    public IEnumerable<int> Holes { get; set; }
    public bool Has9Holes { get; set; }
    public bool HasDeal { get; set; }
    public IEnumerable<PrimaryPrices> PrimaryPrices { get; set; }
    public int PlayersMin { get; set; }
    public int PlayersMax { get; set; }
    public string PlayersDisplay { get; set; }
}

public class GreatLifeRate
{
    public string RatePlanId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
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

