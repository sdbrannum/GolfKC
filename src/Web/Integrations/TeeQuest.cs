using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Web.Dtos;

namespace Web.Integrations;

public interface ITeeQuest : IGolfPlatform
{
}

public class TeeQuest : ITeeQuest
{
    private const string TEE_QUEST_URI = "https://www.teequest.com";
    private const string TEE_QUEST_TEE_TIMES_URI = "https://teetimes.teequest.com";
    private readonly IMemoryCache _cache;
    private readonly CourseOptions _courseOptions;

    public TeeQuest(IOptions<CourseOptions> courseOptions, IMemoryCache cache)
    {
        _cache = cache;
        _courseOptions = courseOptions.Value;
    }

    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.TeeQuest.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://teetimes.teequest.com/{c.Id.Split('-')[0]}"),
            Source = Source.TeeQuest
        });
    }

    /// <summary>
    /// </summary>
    /// <param name="courseId">
    ///     Format should be number-number e.g. 45-2
    ///     If the second number is > 1, then they have multiple courses at the facility
    ///     Example: https://teetimes.teequest.com/49
    /// </param>
    /// <returns></returns>
    public async Task<IEnumerable<TeeTime>> GetTimes(string courseId, DateOnly date)
    {
        var courseIdParts = courseId.Split('-');
        var facilityId = courseIdParts[0];

        return await GetTimesAngleSharp(facilityId, courseId, $"{date.ToString("M/d/yyyy")} 12:00:00 AM");
    }

    private async Task<IEnumerable<TeeTime>> GetTimesAngleSharp(string facilityId, string courseId, string date)
    {
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
        var queryDocument = await context.OpenAsync($"{TEE_QUEST_TEE_TIMES_URI}/{facilityId}");
        var form = queryDocument.QuerySelector<IHtmlFormElement>("form");

        if (form is null) return Enumerable.Empty<TeeTime>();

        var resultDocument = await form.SubmitAsync(new Dictionary<string, string>
        {
            ["Search.Date"] = date,
            ["Search.CourseTag"] = courseId
            // ["Search.Players"] = 
        });

        var teeTimesList = resultDocument.QuerySelectorAll("#tee-times li");

        var teeTimes = teeTimesList.Select(teeTimeItem =>
        {
            var priceContainer = teeTimeItem.QuerySelector(".price-container");
            var holesContainer = teeTimeItem.QuerySelector(".booking-desc");
            var timeContainer = teeTimeItem.QuerySelector(".time-container");
            var playersItem = teeTimeItem.QuerySelector(":nth-last-child(1 of .available)");

            if (priceContainer is null ||
                holesContainer is null ||
                timeContainer is null ||
                playersItem is null)
                return null;

            var parsedPrice = double.TryParse(priceContainer.TextContent.Trim().Replace("$", ""), out var price);
            var parsedHoles = int.TryParse(holesContainer.TextContent.Split(" ")[0], out var holes);
            var parsedTime = TimeOnly.TryParseExact(timeContainer.TextContent.Trim(), "h:mm tt", out var time);
            var parsedPlayers = int.TryParse(playersItem.TextContent.Trim(), out var players);

            if (parsedPrice && parsedHoles && parsedPlayers && parsedTime)
                return new TeeTime
                {
                    Holes = holes,
                    Players = players,
                    Rate = (int) Math.Ceiling(price),
                    Time = time
                };

            return null;
        });

        return teeTimes.Where(t => t is not null)!;
    }
}