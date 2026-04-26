using System.Text;
using System.Text.Json;
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

        var accessToken = await GetCachedAccessToken(clubId);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Result<IEnumerable<TeeTime>>.Fail("ClubProphet: Unable to retrieve api key.");
        }
        
        var transactionId = await RegisterTransaction(clubId);

        if (string.IsNullOrWhiteSpace(transactionId))
        {
            return Result<IEnumerable<TeeTime>>.Fail("ClubProphet: Unable to register transaction id.");       
        }
        
        var teeTimeUri = QueryHelpers.AddQueryString(
            new Uri(baseUri, "onlineres/onlineapi/api/v1/onlinereservation/TeeTimes").ToString(), 
            new Dictionary<string, string?>()
            {
                { "courseIds", cpsCourseId },
                { "searchDate", date.ToString("ddd MMM dd yyyy") },
                { "transactionId", transactionId }
            });

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, teeTimeUri);
        requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
        requestMessage.Headers.Add("x-componentid", "1");
        
        var teeTimesResponse = await _httpClient.SendAsync(requestMessage);

        if (!teeTimesResponse.IsSuccessStatusCode)
        {
            var res = await teeTimesResponse.Content.ReadAsStringAsync();
            return Result<IEnumerable<TeeTime>>.Fail(
                $"ClubProphet: Unable to retrieve tee times for {courseId}.  Responded with ${teeTimesResponse.StatusCode}. ${res}");
        }

        var teeTimesContent = await teeTimesResponse.Content.ReadAsStringAsync();

        try
        {
            var teeTimes = JsonSerializer.Deserialize<ClubProphetTeeTimesResponse>(teeTimesContent);
            var mappedTeeTimes = teeTimes?.Content.Select(TeeTimesMapper.Map) ?? [];
            return Result<IEnumerable<TeeTime>>.Ok(mappedTeeTimes);
        }
        catch (JsonException)
        {
            return Result<IEnumerable<TeeTime>>.Ok([]);
        }
    }

    private async Task<string?> GetCachedAccessToken(string clubId)
    {
        var cacheKey = $"clubprophet:{clubId}:apikey";
    
        var apiKey = await FastCache.Cached.GetOrCompute(
            cacheKey,
            (_) => GetAccessToken(clubId),
            TimeSpan.FromMinutes(3));
    
        return apiKey;
    }

    private async Task<string?> GetAccessToken(string clubId)
    {
        var baseUri = new Uri($"https://{clubId}.cps.golf");
        var optionsUri = new Uri(baseUri,
            $"identityapi/myconnect/token/short");
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, optionsUri);
        requestMessage.Content = new MultipartFormDataContent()
        {
            {new StringContent("onlinereswebshortlived"), "client_id"},
        };
        var optionsResponse = await _httpClient.SendAsync(requestMessage);
        if (!optionsResponse.IsSuccessStatusCode)
        {
            return null;
        }
    
        var tokenResponse = await optionsResponse.Content.ReadFromJsonAsync<ClubProphetToken>();
        return tokenResponse?.AccessToken;
    }

    private async Task<string?> RegisterTransaction(string clubId)
    {
        var transactionId = Guid.NewGuid().ToString();
        var baseUri = new Uri($"https://{clubId}.cps.golf");
        var transactionUri = new Uri(baseUri,
            $"onlineres/onlineapi/api/v1/onlinereservation/RegisterTransactionId");
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, transactionUri);
        var transactionBody = new
        {
            transactionId = transactionId
        };
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(transactionBody), Encoding.UTF8, "application/json");
        requestMessage.Headers.Add("x-componentid", "1");
            
        var optionsResponse = await _httpClient.SendAsync(requestMessage);
        if (!optionsResponse.IsSuccessStatusCode)
        {
            return null;
        }

        return transactionId;
    }
}