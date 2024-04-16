using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Mappers;
using Web.ServiceClients.Dtos;

namespace Web.ServiceClients;

public interface IGreatLife : IGolfPlatform
{
}

public class GreatLife : IGreatLife
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;
    private const string GREAT_LIFE_KC_ID = "20801a91-c14e-42b4-89d2-9eee8a2a23ca";

    public GreatLife(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }

    public Task<IEnumerable<Course>> GetCourses()
    {
        var courses = _courseOptions.GreatLife.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://golfback.com/#/course/{GREAT_LIFE_KC_ID}?courses={c.Id}"),
            Source = Source.GreatLife
        });
        return Task.FromResult(courses);

        // var greatLife = await GetCourseInfo(GREAT_LIFE_KC_ID);
        //
        // if (greatLife is not null)
        // {
        //     var tasks = greatLife.MultiCourseIds.Select(GetCourseInfo);
        //     var courses = await Task.WhenAll(tasks);
        //     return courses.Where(c => c is not null)!;
        // }
        //
        // return Enumerable.Empty<GreatLifeCourse>();
    }

    private async Task<GreatLifeCourse?> GetCourseInfo(string courseId)
    {
        var course = await _httpClient.GetFromJsonAsync<GreatLifeCourseResponse>($"api/v1/courses/{courseId}");
        return course?.Data;
    }

    public async Task<IEnumerable<TeeTime>> GetTimes(string courseId, DateOnly date)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/v1/courses/{courseId}/date/{date.ToString("O")}/teetimes", new
        {
            sessionId = "null"
        });

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<GreatLifeTeeTimesResponse>();
            return data?.TeeTimes?.Select(TeeTimesMapper.Map) ?? Enumerable.Empty<TeeTime>();
        }

        return Enumerable.Empty<TeeTime>();
    }
}