using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Web.Dtos;

namespace Web.Integrations;

public class VermontSystems2 : IVermontSystems
{
    private readonly CourseOptions _courseOptions;

    public VermontSystems2(IOptions<CourseOptions> courseOptions)
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
            Uri = new Uri($"https://{c.Id}.myvscloud.com/webtrac/web/search.html?begintime=06%3A30+am&numberofholes=18&numberofplayers=4&module=gr"),
            Source = Source.VermontSystems
        });
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date)
    {
        var teeTimes = new List<TeeTime>();
        
        var uri = QueryHelpers.AddQueryString($"https://{courseId}.myvscloud.com/webtrac/web/search.html", new Dictionary<string, string?>()
        {
            { "begindate", date.ToString("MM/dd/yyyy") },
            { "numberofholes", "18" },
            { "numberofplayers", "4" },
            { "begintime", "6:30 am" },
            { "module", "GR" },
            { "display", "Detail" }
        });

        var playwright = await Playwright.CreateAsync();
        
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });
        
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
            Locale = "en-US"
        });

        var page = await context.NewPageAsync();

        try
        {
            await page.GotoAsync(uri, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            
            // Check for Cloudflare block
            var title = await page.TitleAsync();
            if (title?.Contains("Cloudflare", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Result<IEnumerable<TeeTime>>.Fail("VermontSystems: Cloudflare blocked");
            }
            
            // Wait for table rows to load
            await page.WaitForSelectorAsync("table tbody tr", new PageWaitForSelectorOptions { Timeout = 15000 });

            var rows = await page.QuerySelectorAllAsync("table tbody tr");

            foreach (var row in rows)
            {
                var timeTxt = await GetInnerText(row, "[data-title='Time']");
                var holesTxt = await GetInnerText(row, "[data-title='Holes']");
                var openSlotsTxt = await GetInnerText(row, "[data-title='Open Slots']");

                var holesStr = holesTxt?.Split(" ").FirstOrDefault();

                var didParseTime = TimeOnly.TryParse(timeTxt, out var time);
                var didParseHoles = int.TryParse(holesStr, out var holes);
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
        }
        catch (TimeoutException)
        {
            return Result<IEnumerable<TeeTime>>.Fail("VermontSystems2: Timeout waiting for page");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TeeTime>>.Fail($"VermontSystems2: {ex.Message}");
        }

        return Result<IEnumerable<TeeTime>>.Ok(teeTimes);
    }

    private static async Task<string?> GetInnerText(IElementHandle element, string selector)
    {
        var handle = await element.QuerySelectorAsync(selector);
        return handle != null ? (await handle.InnerTextAsync())?.Trim() : null;
    }
}