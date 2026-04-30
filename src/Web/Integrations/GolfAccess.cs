using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Extensions;
using Web.Integrations.Dtos;

namespace Web.Integrations;

public interface IGolfAccess : IGolfPlatform {}

public class GolfAccess : IGolfAccess
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;

    public GolfAccess(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }

    public IEnumerable<Course> GetCourses()
    {
        var kcTime = DateTime.UtcNow.ToKc();
        return _courseOptions.GolfAccess.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://golfwithaccess.com/course/{c.Name.ToLowerInvariant()}-golf-club/reserve-tee-time?players=4&startAt=0&endAt=24&date={kcTime:yyyy-MM-dd}"),
            Source = Source.GolfAccess
        });
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        var uri = QueryHelpers.AddQueryString("api/v1/tee-times", new Dictionary<string, string?>()
        {
            { "courseIds", courseId },
            { "startAt", "00:00:00" },
            { "endAt", "23:59:59" },
            { "day", date.ToString("yyyy-MM-dd") },
            { "players", "4" },
        });

        try
        {
            var res = await _httpClient.GetFromJsonAsync<GolfAccessTeeTimeResponse>(uri);

            var teeTimes = res?.TeeTimes.Select(tt => new TeeTime
            {
                Rate = tt.DisplayRate.Price.Dollars.Cents / 100,
                Holes = string.Equals(tt.HolesOption,
                    "EIGHTEEN",
                    StringComparison.OrdinalIgnoreCase)
                    ? 18
                    : 9,
                Players = tt.Players.Max,
                Time = TimeOnly.Parse($"{tt.DayTime.Hour.ToString().PadLeft(2, '0')}:{tt.DayTime.Minute.ToString().PadLeft(2, '0')}:00"),
            });
        
            return Result<IEnumerable<TeeTime>>.Ok(teeTimes ?? []);
        }
        catch
        {
            return Result<IEnumerable<TeeTime>>.Fail();
        }
    }
}