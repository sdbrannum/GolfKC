using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Mappers;
using Web.Integrations.Dtos;

namespace Web.Integrations;

public interface IGolfBack : IGolfPlatform
{
}

public class GolfBack : IGolfBack
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;
    // private const string GREAT_LIFE_KC_ID = "20801a91-c14e-42b4-89d2-9eee8a2a23ca";

    public GolfBack(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }

    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.GolfBack.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://golfback.com/#/course/{c.Id}"),
            Source = Source.GolfBack
        });
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/courses/{courseId}/date/{date.ToString("O")}/teetimes", new
        {
            sessionId = "null"
        });

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<GolfBackTeeTimesResponse>();
            var teeTimes = data?.TeeTimes?.Select(TeeTimesMapper.Map) ?? Enumerable.Empty<TeeTime>();
            return Result<IEnumerable<TeeTime>>.Ok(teeTimes);
        }
        var res = await response.Content.ReadAsStringAsync();
        return Result<IEnumerable<TeeTime>>.Fail(
            $"GolfBack: Unable to retrieve tee times for {courseId}.  Responded with ${response.StatusCode}. ${res}");
    }
}