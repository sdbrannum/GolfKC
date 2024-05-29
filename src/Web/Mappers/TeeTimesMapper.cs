using Web.Dtos;
using Web.Integrations;
using Web.Integrations.Dtos;

namespace Web.Mappers;

public static class TeeTimesMapper
{
    public static TeeTime Map(GreatLifeTeeTime teeTime)
    {
        var rate = teeTime.Rates.First();
        return new TeeTime
        {
            Rate = (int) Math.Ceiling(rate.Price),
            Players = teeTime.PlayersMax,
            Time = TimeOnly.FromDateTime(teeTime.LocalDateTime),
            Holes = teeTime.Holes.Max()
        };
    }

    public static TeeTime Map(GolfNowTeeTime teeTime)
    {
        return new TeeTime
        {
            Rate = (int)Math.Ceiling(teeTime.DisplayRate),
            Players = MapPlayersFromRule(teeTime.PlayerRule),
            Time = TimeOnly.FromDateTime(teeTime.Time),
            Holes = int.Parse(teeTime.MultipleHolesRate)
        };
    }
    
    public static TeeTime Map(ChronoTeeTime teeTime)
    {
        return new TeeTime
        {
            Rate = (int)Math.Ceiling(teeTime.GreenFees.First().Price),
            Players = teeTime.GreenFees.Count(),
            Time = TimeOnly.Parse(teeTime.Time),
            Holes = 18
        };
    }

    private static int MapPlayersFromRule(int playerRule)
    {
        return playerRule switch
        {
            1 => 1,
            3 => 2,
            14 => 4,
            15 => 4,
            _ => playerRule
        };
    }

    public static TeeTime Map(ForeUpTeeTime teeTime)
    {
        var is18 = teeTime.AvailableSpots18 > 0;
        return new TeeTime
        {
            Rate = is18 ? (teeTime.GreenFee18 + teeTime.CartFee18) : (teeTime.GreenFee9 + teeTime.CartFee9),
            Players = is18 ? teeTime.AvailableSpots18 : teeTime.AvailableSpots9,
            Time = TimeOnly.Parse(teeTime.Time.Split(" ")[1]),
            Holes = is18 ? 18 : 9
        };
    }
}