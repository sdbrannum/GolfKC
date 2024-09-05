using Web.Dtos;
using Web.Integrations;
using Web.Integrations.Dtos;

namespace Web.Mappers;

public static class TeeTimesMapper
{
    public static TeeTime Map(GolfBackTeeTime teeTime)
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
            7 => 3,
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

    public static TeeTime Map(ClubProphetTeeTime teeTime)
    {
        var is18 = teeTime.Holes > 9;
        var cartFeeCodes = is18
            ? new[] {ClubProphetTeeTime.ShItemCode.FullCart18Online, ClubProphetTeeTime.ShItemCode.FullCart18}
            : new[] {ClubProphetTeeTime.ShItemCode.FullCart9Online, ClubProphetTeeTime.ShItemCode.FullCart9};

        var greenFeeCodes = is18
            ? new[] {ClubProphetTeeTime.ShItemCode.GreenFee18Online, ClubProphetTeeTime.ShItemCode.GreenFee18 }
            : new[] { ClubProphetTeeTime.ShItemCode.GreenFee9Online, ClubProphetTeeTime.ShItemCode.GreenFee9 };

        var cartFee = teeTime.ShItemPrices.FirstOrDefault(c => cartFeeCodes.Any(cfc => cfc == c.ShItemCode))?.Price;
        var greenFee = teeTime.ShItemPrices.FirstOrDefault(c => greenFeeCodes.Any(gfc => gfc == c.ShItemCode))?.Price;
        
        return new TeeTime
        {
            Rate = (int)((cartFee ?? 0.0) + (greenFee ?? 0.0)),
            Players = teeTime.MaxPlayer,
            Holes = teeTime.Holes,
            Time = TimeOnly.FromDateTime(teeTime.StartTime),
        };
    }

    public static TeeTime Map(TeeItUpTeeTime teeTime)
    {
        var centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(teeTime.Time, "Central Standard Time");
        var rate = teeTime.Rates.MaxBy(r => r.Holes);
        
        return new TeeTime
        {
            Players = teeTime.MaxPlayers,
            Time = TimeOnly.FromDateTime(centralDateTime),
            Holes = rate?.Holes ?? -1,
            Rate = (int)(rate?.GreenFeeCart ?? 0) / 100
        };
    }
}