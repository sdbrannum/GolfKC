using System.Text.Json.Serialization;

namespace Web.ServiceClients.Dtos;

public class ChronoTeeTime
{
    [JsonPropertyName("start_time")]
    public string Time { get; set; } = default!;
    public string Date { get; set; } = default!;
    [JsonPropertyName("out_of_capacity")]
    public bool OutOfCapacity { get; set; }
    [JsonPropertyName("green_fees")] 
    public IEnumerable<ChronoGreenFee> GreenFees { get; set; } = default!;
}

public class ChronoGreenFee
{
    public double Price { get; set; }
}

