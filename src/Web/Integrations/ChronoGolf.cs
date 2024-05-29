using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Mappers;
using Web.Integrations.Dtos;

namespace Web.Integrations;

public interface IChronoGolf : IGolfPlatform
{
}

public class ChronoGolf : IChronoGolf
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;

    public ChronoGolf(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }

    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.ChronoGolf.Select(c =>
        {
            var course = _courseOptions.ChronoGolf.Single(cg => cg.Id == c.Id);
            var clubId = course.Id.Split('-')[0];
            var chronoCourseId = course.Id.Split('-')[1];
            return new Course
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                Uri = new Uri(
                    $"https://www.chronogolf.com/club/{clubId}/widget?medium=widget&source=club#?course_id={chronoCourseId}&nb_holes=18"),
                Source = Source.Chrono
            };
        });
    }

    public async Task<IEnumerable<TeeTime>> GetTimes(string courseId, DateOnly date)
    {
        var course = _courseOptions.ChronoGolf.FirstOrDefault(c => c.Id == courseId);
        if (course is null)
        {
            return Enumerable.Empty<TeeTime>();
        }

        var clubId = course.Id.Split('-')[0];
        var chronoCourseId = course.Id.Split('-')[1];

        var formattedDate = date.ToString("O");
        var playersQuery = $"affiliation_type_ids[]={course.DailyFeeId}";
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<ChronoTeeTime>>($"marketplace/clubs/{clubId}/teetimes?date={formattedDate}&course_id={chronoCourseId}&nb_holes=18&{playersQuery}&{playersQuery}&{playersQuery}&{playersQuery}");
        if (response is null)
        {
            return Enumerable.Empty<TeeTime>();
        }

        return response.Where(tt => !tt.OutOfCapacity)
            .Select(TeeTimesMapper.Map);
    }
}