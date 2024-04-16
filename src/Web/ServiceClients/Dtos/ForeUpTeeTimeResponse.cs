using System.Text.Json.Serialization;

namespace Web.ServiceClients.Dtos;

public class ForeUpTeeTime
{
    /// <remarks>
    /// Format: 2024-04-10 08:20
    /// </remarks>
    public string Time { get; init; } = default!;

    [JsonPropertyName("Available_spots")] 
    public int AvailableSpots { get; init; }

    [JsonPropertyName("available_spots_9")]
    public int AvailableSpots9 { get; init; }

    [JsonPropertyName("available_spots_18")]
    public int AvailableSpots18 { get; init; }

    [JsonPropertyName("green_fee")] 
    public int GreenFee { get; init; }
    [JsonPropertyName("green_fee_9")] 
    public int GreenFee9 { get; init; }
    [JsonPropertyName("green_fee_18")] 
    public int GreenFee18 { get; init; }
    [JsonPropertyName("cart_fee")] 
    public int CartFee { get; init; }
    [JsonPropertyName("cart_fee_9")] 
    public int CartFee9 { get; init; }
    [JsonPropertyName("cart_fee_18")] 
    public int CartFee18 { get; init; }
}