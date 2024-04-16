namespace Web.Dtos;

public class CourseOptions
{
    public const string Courses = "Courses";

    public IEnumerable<CourseOption> GolfNow { get; init; } = new List<CourseOption>();
    public IEnumerable<CourseOption> GreatLife { get; init; } = new List<CourseOption>();
    public IEnumerable<CourseOption> TeeQuest { get; init; } = new List<CourseOption>();
    
    public IEnumerable<CourseOption> ForeUp { get; init; } = new List<CourseOption>();
    public IEnumerable<ChronoGolfOption> ChronoGolf { get; init; } = new List<ChronoGolfOption>();

    public class CourseOption
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public Address Address { get; init; } = default!;
    }
    public class ChronoGolfOption : CourseOption
    {
        public uint DailyFeeId { get; init; } = default!;
    }
}