namespace Web.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToKc(this DateTime date)
    {
        var americaChicago = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        return TimeZoneInfo.ConvertTimeFromUtc(date, americaChicago);
    }
}