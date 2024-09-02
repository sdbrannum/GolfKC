using AngleSharp;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Web.Dtos;

namespace Web.Integrations;

public interface IVermontSystems : IGolfPlatform
{
}

public class VermontSystems : IVermontSystems
{
    private readonly CourseOptions _courseOptions;

    public VermontSystems(IOptions<CourseOptions> courseOptions)
    {
        _courseOptions = courseOptions.Value;
    }
    
    public IEnumerable<Course> GetCourses()
    {
        return _courseOptions.VermontSystems.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://web2.myvscloud.com/wbwsc/{c.Id}/search.html?display=Detail&module=GR"),
            Source = Source.VermontSystems
        });
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
        var uri = QueryHelpers.AddQueryString($"https://web2.myvscloud.com/wbwsc/{courseId}/search.html", new Dictionary<string, string?>()
        {
            {"begindate", date.ToString("MM/dd/yyyy") },
            { "numberofholes", "18" },
            { "numberofplayers", "4" },
            { "begintime", "6:00 am" },
            { "module", "GR" },
            { "display", "Detail" }
        });
        var queryDocument = await context.OpenAsync(uri);
        var timeRows = queryDocument.QuerySelectorAll("table tbody tr");

        var teeTimes = new List<TeeTime>();
        foreach (var timeRow in timeRows)
        {
            // format 7:40 am
            var timeTxt = timeRow.QuerySelector("[data-title='Time']")?.TextContent.Trim();
            // format 06/18/2024
            // var dateTxt = timeRow.QuerySelector("[data-title='Date']")?.TextContent.Trim();
            // possible format 18 (Front)
            var holesTxt = timeRow.QuerySelector("[data-title='Holes']")?.TextContent.Trim().Split(" ")
                .FirstOrDefault();
            var openSlotsTxt = timeRow.QuerySelector("[data-title='Open Slots']")?.TextContent.Trim();

            var didParseTime = TimeOnly.TryParse(timeTxt, out var time);
            var didParseHoles = int.TryParse(holesTxt, out var holes);
            var didParseOpenSlots = int.TryParse(openSlotsTxt, out var openSlots);

            if (didParseTime && didParseHoles && didParseOpenSlots && openSlots > 0)
            {
                teeTimes.Add(new TeeTime
                {
                    Time = time,
                    Players = openSlots,
                    Holes = holes,
                    Rate = -1
                });
            }
        }

        return Result<IEnumerable<TeeTime>>.Ok(teeTimes);
    }
}