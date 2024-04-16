namespace Web.Dtos;

public sealed class TeeTime
{
    /// <summary>
    /// Green fee + cart fee rounded up
    /// </summary>
    public int Rate { get; init; }
    public int Players { get; init; }
    public TimeOnly Time { get; init; }
    public int Holes { get; init; }
}