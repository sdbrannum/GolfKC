using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Web.Dtos;
using Web.Integrations.Dtos;
using Web.Mappers;

namespace Web.Integrations;

public interface IClubProphet : IGolfPlatform {}

public class ClubProphet : IClubProphet
{
    private readonly HttpClient _httpClient;
    private readonly CourseOptions _courseOptions;

    public ClubProphet(HttpClient httpClient, IOptions<CourseOptions> courseOptions)
    {
        _httpClient = httpClient;
        _courseOptions = courseOptions.Value;
    }
    
    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.ClubProphet.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://{c.Id.Split(':')[0]}.cps.golf/onlineresweb/search-teetime?TeeOffTimeMin=0&TeeOffTimeMax=23"),
            Source = Source.ClubProphet
        });
    }

    public async Task<IEnumerable<TeeTime>> GetTimes(string courseId, DateOnly date)
    {
        var idParts = courseId.Split(':');
        var clubId = idParts[0];
        var cpsCourseId = idParts[1];

        var baseUri = new Uri($"https://{clubId}.cps.golf");
        
        var config =
            await _httpClient.GetFromJsonAsync<ClubProphetConfig>(new Uri(baseUri, "onlineresweb/Home/Configuration"));

        if (string.IsNullOrWhiteSpace(config?.ApiKey))
        {
            return Enumerable.Empty<TeeTime>();
        }
        
        var teeTimeUri = QueryHelpers.AddQueryString(
            new Uri(baseUri, "onlineres/onlineapi/api/v1/onlinereservation/TeeTimes").ToString(), 
            new Dictionary<string, string?>()
            {
                { "courseIds", cpsCourseId },
                { "searchDate", date.ToString("ddd MMM dd yyyy") },
            });

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, teeTimeUri);
        requestMessage.Headers.Add("x-apikey", config.ApiKey);
        requestMessage.Headers.Add("x-componentid", "1");
        var teeTimesResponse = await _httpClient.SendAsync(requestMessage);

        if (!teeTimesResponse.IsSuccessStatusCode)
        {
            return Enumerable.Empty<TeeTime>();
        }

        var teeTimes = await teeTimesResponse.Content.ReadFromJsonAsync<IEnumerable<ClubProphetTeeTime>>();

        return teeTimes?.Select(TeeTimesMapper.Map) ?? Enumerable.Empty<TeeTime>();
    }
}