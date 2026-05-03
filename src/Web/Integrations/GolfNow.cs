using System.Globalization;
using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Mappers;
using Web.Integrations.Dtos;

namespace Web.Integrations;

public interface IGolfNow : IGolfPlatform
{
}

public class GolfNow : IGolfNow
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;

    public GolfNow(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }

    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.GolfNow.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri("https://www.golfnow.com/"),
            Source = Source.GolfNow
        });
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        var formattedDate = date.ToString("MMMM dd yyyy", new CultureInfo("en-us"));
        var response = await _httpClient.PostAsJsonAsync("api/tee-times/tee-time-search-results", new GolfNowTeeTimesRequest
        {
            FacilityId = courseId,
            Date = formattedDate,
        });
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<GolfNowResponse>();
            var teeTimes = data?.Data.TeeTimes?.Select(TeeTimesMapper.Map) ?? Enumerable.Empty<TeeTime>();
            return Result<IEnumerable<TeeTime>>.Ok(teeTimes);
        }
        
        var res = await response.Content.ReadAsStringAsync();
        return Result<IEnumerable<TeeTime>>.Fail(
            $"GolfNow: Unable to retrieve tee times for {courseId}.  Responded with ${response.StatusCode}. ${res}");
    }
}