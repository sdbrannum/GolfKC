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

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        var idParts = courseId.Split(':');
        var clubId = idParts[0];
        var cpsCourseId = idParts[1];

        var baseUri = new Uri($"https://{clubId}.cps.golf");

        var config = await GetConfig(clubId);

        if (string.IsNullOrWhiteSpace(config.ApiKey))
        {
            return Result<IEnumerable<TeeTime>>.Fail("ClubProphet: Unable to retrieve api key.");
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
        // website id will get us more tee times occasionally, i have no idea why
        requestMessage.Headers.Add("x-websiteid", config.WebsiteId);
        var teeTimesResponse = await _httpClient.SendAsync(requestMessage);

        if (!teeTimesResponse.IsSuccessStatusCode)
        {
            var res = await teeTimesResponse.Content.ReadAsStringAsync();
            return Result<IEnumerable<TeeTime>>.Fail(
                $"ClubProphet: Unable to retrieve tee times for {courseId}.  Responded with ${teeTimesResponse.StatusCode}. ${res}");
        }

        var teeTimes = await teeTimesResponse.Content.ReadFromJsonAsync<IEnumerable<ClubProphetTeeTime>>();

        var mappedTeeTimes = teeTimes?.Select(TeeTimesMapper.Map) ?? Enumerable.Empty<TeeTime>();
        return Result<IEnumerable<TeeTime>>.Ok(mappedTeeTimes);
    }

    private async Task<(string? ApiKey, string? WebsiteId)> GetConfig(string clubId)
    {
        var baseUri = new Uri($"https://{clubId}.cps.golf");
        
        var config = await FastCache.Cached.GetOrCompute(
            $"clubprophet:{clubId}:config", 
            (_) => _httpClient.GetFromJsonAsync<ClubProphetConfig>(new Uri(baseUri, "onlineresweb/Home/Configuration")),
            TimeSpan.FromMinutes(3));

        if (string.IsNullOrWhiteSpace(config?.ApiKey))
        {
            return (null, null);
        }
        
        var options = await FastCache.Cached.GetOrCompute(
            $"clubprophet:{clubId}:options",
            (_) => GetOptions(clubId, config.ApiKey),
            TimeSpan.FromMinutes(3));

        return (config?.ApiKey, options?.WebsiteId);
    }

    private async Task<ClubProphetOptions?> GetOptions(string clubId, string apiKey)
    {
        var baseUri = new Uri($"https://{clubId}.cps.golf");
        var optionsUri = new Uri(baseUri,
            $"onlineres/onlineapi/api/v1/onlinereservation/GetAllOptions/{clubId}");
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, optionsUri);
        requestMessage.Headers.Add("x-apikey", apiKey);
        requestMessage.Headers.Add("x-componentid", "1");
        var optionsResponse = await _httpClient.SendAsync(requestMessage);

        if (!optionsResponse.IsSuccessStatusCode)
        {
            return null;
        }

        return await optionsResponse.Content.ReadFromJsonAsync<ClubProphetOptions>();
    }
}