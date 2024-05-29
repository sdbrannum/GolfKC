using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Mappers;
using Web.Integrations.Dtos;

namespace Web.Integrations;

public interface IForeUp : IGolfPlatform {}

public class ForeUp : IForeUp
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;
    private const string URI_COURSE = "https://foreupsoftware.com/index.php/booking/{0}#/teetimes";
    private const string URI_API_TIMES = "https://foreupsoftware.com/index.php/api/booking/times";

    public ForeUp(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }
    
    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.ForeUp.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri(string.Format(URI_COURSE, c.Id)),
            Source = Source.ForeUp
        });
    }

    public async Task<IEnumerable<TeeTime>> GetTimes(string courseId, DateOnly date)
    {
        // we have parse the HTML and find the DEFAULT_FILTER to get the schedule_id for tee time requests
        var scheduleId = await GetScheduleId(courseId);

        if (scheduleId is null)
        {
            return Enumerable.Empty<TeeTime>();
        }
        var queryParams = new Dictionary<string, string?>()
        {
            { "time", "all" },
            { "date", date.ToString("M-dd-yyyy") },
            { "schedule_id", scheduleId.Value.ToString() },
            { "api_key", "no_limits" }
        };

        var apiUri = new Uri(QueryHelpers.AddQueryString(URI_API_TIMES, queryParams));
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<ForeUpTeeTime>>(apiUri);

        if (response is null)
        {
            return Enumerable.Empty<TeeTime>();
        }

        return response.Select(TeeTimesMapper.Map);
    }

    /// <summary>
    /// Find the schedule id for the course to get tee times
    /// </summary>
    /// <remarks>
    /// The schedule_id is embedded in a script tag in the DEFAULT_FILTER variable
    /// </remarks>
    /// <param name="courseId"></param>
    /// <returns></returns>
    private async Task<int?> GetScheduleId(string courseId)
    {
        // we have parse the HTML and find the DEFAULT_FILTER to get the schedule_id for tee time requests
        var courseInfo = await _httpClient.GetStringAsync(string.Format(URI_COURSE, courseId));
        
        var parts = courseInfo.Split("DEFAULT_FILTER");
        var split = parts.LastOrDefault();
        var firstIndex = split?.IndexOf(':');
        var secondIndex = split?.IndexOf(',');

        if (split is null || firstIndex is null || secondIndex is null)
        {
            return null;
        }

        var possibleScheduleId = split.Substring(firstIndex.Value + 1, secondIndex.Value - (firstIndex.Value + 1));
        var didParse = int.TryParse(possibleScheduleId, out var scheduleId);

        return didParse ? scheduleId : null;
    }
}