using System.Text.Json.Serialization;

namespace Web.Integrations.Dtos;

public class ClubProphetTeeTime
{
    public DateTime StartTime { get; init; }
    public int MaxPlayer { get; init; }
    public int Holes { get; init; }
    public IEnumerable<Rate> ShItemPrices { get; init; }

    public class Rate
    {
        public ShItemCode ShItemCode { get; init; }
        public double Price { get; init; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShItemCode
    {
        Unknown = 0,
        GreenFee18,
        GreenFee18Online,
        FullCart18,
        FullCart18Online,
        GreenFee9,
        GreenFee9Online,
        FullCart9,
        FullCart9Online
    }
}