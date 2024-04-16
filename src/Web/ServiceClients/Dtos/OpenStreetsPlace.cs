using System.Text.Json.Serialization;

namespace Web.ServiceClients.Dtos;

public sealed class OpenStreetsPlace
{
    [JsonPropertyName("place_id")]
    public int PlaceId { get; init; }
    public string Licence { get; init; }
    [JsonPropertyName("lat")]
    public string Latitude { get; init; }
    [JsonPropertyName("lon")]
    public string Longitude { get; init; }
    public string Category { get; init; }
    public string Type { get; init; }
    [JsonPropertyName("place_rank")]
    public int PlaceRank { get; init; }
    public double Importance { get; init; }
    [JsonPropertyName("addresstype")]
    public string AddressType { get; init; }
    public string Name { get; init; }
    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; }
    [JsonPropertyName("boundingbox")]
    public string[] BoundingBox { get; init; }
}

