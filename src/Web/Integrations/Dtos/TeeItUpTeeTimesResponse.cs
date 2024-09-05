using System.Text.Json.Serialization;

namespace Web.Integrations.Dtos;

public class TeeItUpTeeTimesResponse
{
    [JsonPropertyName("teetimes")]
    public IEnumerable<TeeItUpTeeTime> Times { get; init; } = new List<TeeItUpTeeTime>();
}

public class TeeItUpTeeTime
{
    // in UTC
    [JsonPropertyName("teetime")]
    public DateTime Time { get; init; }
    [JsonPropertyName("maxPlayers")]
    public int MaxPlayers { get; init; }
    [JsonPropertyName("rates")]
    public IEnumerable<TeeItUpRate> Rates { get; init; } = new List<TeeItUpRate>();
}

public class TeeItUpRate
{
    // in cents
    [JsonPropertyName("greenFeeCart")]
    public double GreenFeeCart { get; init; }
    [JsonPropertyName("holes")]
    public int Holes { get; init; }
}