using Web.Dtos;

namespace Web.Integrations.Dtos;

public class GolfAccessTeeTimeResponse
{
    public IEnumerable<GolfAccessTeeTime> TeeTimes { get; set; }
}

public class GolfAccessTeeTime
{
    public GolfAccessDayTime DayTime { get; init; }
    public GolfAccessPlayers Players { get; init; }
    public GolfAccessDisplayRate DisplayRate { get; init; }
    public string HolesOption { get; init; }
    
    public class GolfAccessDayTime
    {
        public int Hour { get; init; }
        public int Minute { get; init; }
    }

    public class GolfAccessPlayers
    {
        public int Min { get; init; }
        public int Max { get; init; }
    }

    public class GolfAccessDisplayRate
    {
        public GolfAccessPrice Price { get; init; }
    }

    public class GolfAccessPrice
    {
        public GolfAccessDollars Dollars { get; init; }
    }

    public class GolfAccessDollars
    {
        public int Cents { get; init; }
    }
}