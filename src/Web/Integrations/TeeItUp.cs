using System.Text.Json;
using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Integrations.Dtos;
using Web.Mappers;

namespace Web.Integrations;

public interface ITeeItUp : IGolfPlatform
{
}

public class TeeItUp : ITeeItUp
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;

    public TeeItUp(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }
    
    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.TeeItUp.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://{c.Id.Split(':')[1]}.book.teeitup.golf?course={c.Id.Split(':')[0]}"),
            Source = Source.TeeItUp
        });
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        try
        {
            var courseIdParts = courseId.Split(':');
            var courseIdentifier = courseIdParts[0];
            var courseAlias = courseIdParts[1];
            // https://phx-api-be-east-1b.kenna.io/v2/tee-times?date=2024-09-09&facilityIds=3902
            var formattedDate = date.ToString("O");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/v2/tee-times?date={formattedDate}&facilityIds={courseIdentifier}");
            requestMessage.Headers.Add("x-be-alias", courseAlias);
            var teeTimesResponse = await _httpClient.SendAsync(requestMessage);
        
            if (!teeTimesResponse.IsSuccessStatusCode)
            {
                var res = await teeTimesResponse.Content.ReadAsStringAsync();
                return Result<IEnumerable<TeeTime>>.Fail(
                    $"TeeItUp: Unable to retrieve tee times for {courseId}.  Responded with ${teeTimesResponse.StatusCode}. ${res}");
            }

        
            var content = await teeTimesResponse.Content.ReadAsStringAsync();
            var teeTimesJson = JsonSerializer.Deserialize<IEnumerable<TeeItUpTeeTimesResponse>>(content);

            var mappedTeeTimes = teeTimesJson?.FirstOrDefault()?.Times.Select(TeeTimesMapper.Map) ?? Enumerable.Empty<TeeTime>();
            return Result<IEnumerable<TeeTime>>.Ok(mappedTeeTimes);
        }
        catch (Exception e)
        {
            return Result<IEnumerable<TeeTime>>.Fail(
                $"TeeItUp: Unable to parse tee times for {courseId}. ${e.Message}.");
        }
    }
}