using System.Globalization;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Web.Dtos;
using Web.ServiceClients.Dtos;

namespace Web.ServiceClients;

public interface ITeeQuest : IGolfPlatform
{
}

public class TeeQuest : ITeeQuest
{
    private readonly IMemoryCache _cache;
    private const string TEE_QUEST_URI = "https://www.teequest.com";
    private const string TEE_QUEST_TEE_TIMES_URI = "https://teetimes.teequest.com";
    private readonly CourseOptions _courseOptions;

    public TeeQuest(IOptions<CourseOptions> courseOptions, IMemoryCache cache)
    {
        _cache = cache;
        _courseOptions = courseOptions.Value;
    }

    public Task<IEnumerable<Course>> GetCourses()
    {
        var courses = _courseOptions.TeeQuest.Select(c => new Course
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            Uri = new Uri($"https://teetimes.teequest.com/{c.Id.Split('-')[0]}"),
            Source = Source.TeeQuest
        });

        return Task.FromResult(courses);

        // using var playwright = await Playwright.CreateAsync();
        // await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        // {
        //     Args = new[] { "--disable-gl-drawing-for-tests" }
        // });
        // var page = await browser.NewPageAsync();
        // await page.GotoAsync(TEE_QUEST_URI);
        // var courseAreaSelector = await page.QuerySelectorAsync("#cmbCourses");
        // if (courseAreaSelector is not null)
        // {
        //     var kcOptions = await courseAreaSelector.QuerySelectorAllAsync("optgroup[label*='Kansas'] option");
        //     var courseTasks = kcOptions.Select(async opt =>
        //     {
        //         var name = await opt.InnerTextAsync();
        //         var id = await opt.GetAttributeAsync("value");
        //         return (id, name);
        //     }).ToList();
        //     await Task.WhenAll(courseTasks);
        //     return courseTasks
        //         .Where(v => !string.IsNullOrWhiteSpace(v.Result.id) && !string.IsNullOrWhiteSpace(v.Result.name))
        //         .Select(ct => new TeeQuestCourse()
        //         {
        //             Id = ct.Result.id!,
        //             Name = ct.Result.name!
        //         });
        // }
        // return Enumerable.Empty<TeeQuestCourse>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="courseId">
    /// Format should be number-number e.g. 45-2
    /// If the second number is > 1, then they have multiple courses at the facility
    /// Example: https://teetimes.teequest.com/49
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

        if (form is null)
        {
            return Enumerable.Empty<TeeTime>();
        }

        var resultDocument = await form.SubmitAsync(new Dictionary<string, string>()
        {
            ["Search.Date"] = date,
            ["Search.CourseTag"] = courseId,
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
            {
                return null;
            }

            var parsedPrice = double.TryParse(priceContainer.TextContent.Trim().Replace("$", ""), out var price);
            var parsedHoles = int.TryParse(holesContainer.TextContent.Split(" ")[0], out var holes);
            var parsedTime = TimeOnly.TryParseExact(timeContainer.TextContent.Trim(), "h:mm tt", out var time);
            var parsedPlayers = int.TryParse(playersItem.TextContent.Trim(), out var players);

            if (parsedPrice && parsedHoles && parsedPlayers && parsedTime)
            {
                return new TeeTime
                {
                    Holes = holes,
                    Players = players,
                    Rate = (int)Math.Ceiling(price),
                    Time = time,
                };
            }

            return null;
        });

        return teeTimes.Where(t => t is not null)!;
    }


    // private async Task<TeeTime?> GetTeeTimePlaywright(IElementHandle teeTimeHandle)
    // {
    //     var priceContainerTask = teeTimeHandle.QuerySelectorAsync(".price-container");
    //     var holesContainerTask = teeTimeHandle.QuerySelectorAsync(".booking-desc");
    //     var timeContainerTask = teeTimeHandle.QuerySelectorAsync(".time-container");
    //     var playersItemTask = teeTimeHandle.QuerySelectorAsync(":nth-last-child(1 of .available)");
    //
    //     await Task.WhenAll(priceContainerTask, holesContainerTask, timeContainerTask, playersItemTask);
    //
    //     if (priceContainerTask.Result is null ||
    //         holesContainerTask.Result is null ||
    //         timeContainerTask.Result is null ||
    //         playersItemTask.Result is null)
    //     {
    //         return null;
    //     }
    //
    //     var priceTextTask = priceContainerTask.Result.TextContentAsync();
    //     var holesTextTask = holesContainerTask.Result.TextContentAsync();
    //     var timeTextTask = timeContainerTask.Result.TextContentAsync();
    //     var playersTextTask = playersItemTask.Result.TextContentAsync();
    //
    //     await Task.WhenAll(priceTextTask, holesTextTask, timeTextTask, playersTextTask);
    //
    //     if (string.IsNullOrWhiteSpace(priceTextTask.Result) ||
    //         string.IsNullOrWhiteSpace(holesTextTask.Result) ||
    //         string.IsNullOrWhiteSpace(timeTextTask.Result) ||
    //         string.IsNullOrWhiteSpace(playersTextTask.Result))
    //     {
    //         return null;
    //     }
    //
    //     var parsedPrice = double.TryParse(priceTextTask.Result.Trim().Replace("$", ""), out var price);
    //     var parsedHoles = uint.TryParse(holesTextTask.Result.Split(" ")[0], out var holes);
    //     var parsedPlayers = uint.TryParse(playersTextTask.Result.Trim(), out var players);
    //     var parsedTime = TimeOnly.TryParseExact(timeTextTask.Result.Trim(), "h:mm tt", out var time);
    //
    //     if (parsedPrice && parsedHoles && parsedPlayers && parsedTime)
    //     {
    //         return new TeeTime
    //         {
    //             Holes = holes,
    //             Players = players,
    //             Rate = price,
    //             Time = time,
    //         };
    //     }
    //
    //     return null;
    // }

 
    // public async Task<IEnumerable<TeeTime>> GetTimesPlaywright(string courseId, DateOnly date)
    // {
    //     var courseIdParts = courseId.Split('-');
    //     var facilityId = courseIdParts[0];
    //     var hasCourseNumber = uint.TryParse(courseIdParts[1], out var courseNumber);
    //     
    //     using var playwright = await Playwright.CreateAsync();
    //     await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
    //     {
    //         Args = new[] { "--disable-gl-drawing-for-tests" }
    //     });
    //     var page = await browser.NewPageAsync();
    //     await page.GotoAsync($"{TEE_QUEST_TEE_TIMES_URI}/{facilityId}");
    //
    //     if (hasCourseNumber && courseNumber > 1)
    //     {
    //         var courseSelector = await page.QuerySelectorAsync("#Search_CourseTag");
    //         if (courseSelector is not null)
    //         {
    //             await courseSelector.SelectOptionAsync(courseId);
    //         }
    //     }
    //
    //     var dateSelector = await page.QuerySelectorAsync("#Search_Date");
    //
    //     if (dateSelector is null)
    //     {
    //         return Enumerable.Empty<TeeTime>();
    //     }
    //
    //     var formattedDate = date.ToString("M/d/yyyy");
    //     var option = await dateSelector.QuerySelectorAsync($"option[value*='{formattedDate}']");
    //
    //     if (option is null)
    //     {
    //         return Enumerable.Empty<TeeTime>();
    //     }
    //
    //     var optionValue = await option.GetAttributeAsync("value");
    //     if (optionValue is null)
    //     {
    //         return Enumerable.Empty<TeeTime>();
    //     }
    //
    //     await dateSelector.SelectOptionAsync(optionValue);
    //
    //     var submitBtn = await page.QuerySelectorAsync("input[type='submit']");
    //
    //     if (submitBtn is null)
    //     {
    //         return Enumerable.Empty<TeeTime>();
    //     }
    //
    //     await submitBtn.ClickAsync();
    //     try
    //     {
    //         // waits 30 seconds and then throws an exception
    //         await page.WaitForSelectorAsync("#tee-times", new PageWaitForSelectorOptions
    //         {
    //             State = WaitForSelectorState.Attached,
    //         });
    //     }
    //     catch (TimeoutException)
    //     {
    //         return Enumerable.Empty<TeeTime>();
    //     }
    //
    //     var teeTimesItems = await page.QuerySelectorAllAsync("#tee-times li");
    //
    //     var teeTimeTasks = teeTimesItems.Select(GetTeeTimePlaywright).ToList();
    //     await Task.WhenAll(teeTimeTasks);
    //     return teeTimeTasks.Select(tt => tt.Result).Where(tt => tt is not null)!;
    // }
}