using System.Globalization;
using Web.Dtos;
using Web.ServiceClients;

namespace Web;

public class GolfService : IGolfService
{
    private readonly IGreatLife _greatLife;
    private readonly IGolfNow _golfNow;
    private readonly ITeeQuest _teeQuest;
    private readonly IChronoGolf _chronoGolf;
    private readonly IForeUp _foreUp;
    private readonly IOpenStreets _openStreets;
    private uint RADIUS = 30;
    private const double LATITUDE = 39.01470395078284;
    private const double LONGITUDE = -94.69324882561136;
    private readonly HashSet<string> _states = new() {"MO", "KS"};

    public GolfService(
        IGreatLife greatLife,
        IGolfNow golfNow,
        ITeeQuest teeQuest,
        IChronoGolf chronoGolf,
        IForeUp foreUp,
        IOpenStreets openStreets)
    {
        _greatLife = greatLife;
        _golfNow = golfNow;
        _teeQuest = teeQuest;
        _chronoGolf = chronoGolf;
        _foreUp = foreUp;
        _openStreets = openStreets;
    }

    public async Task<IEnumerable<Course>> GetCourses(DateOnly date)
    {
        var formattedDate = date.ToString("MMMM dd yyyy", new CultureInfo("en-us"));
        var golfNowCourses = await _golfNow.GetCourses();
        var foreUpCourses = await _foreUp.GetCourses();
        var greatLifeCourses = await _greatLife.GetCourses();
        var teeQuestCourses = await _teeQuest.GetCourses();
        var chronoCourses = await _chronoGolf.GetCourses();

        return golfNowCourses.Concat(foreUpCourses)
            .Concat(greatLifeCourses)
            .Concat(teeQuestCourses)
            .Concat(chronoCourses);
    }

    public async Task<IEnumerable<TeeTime>> GetTeeTimes(Source source, string courseId, DateOnly date)
    {
        if (source == Source.GreatLife)
        {
            return await _greatLife.GetTimes(courseId, date);
        }

        if (source == Source.GolfNow)
        {
            return await _golfNow.GetTimes(courseId, date);
        }

        if (source == Source.Chrono)
        {
            return await _chronoGolf.GetTimes(courseId, date);
        }

        if (source == Source.ForeUp)
        {
            return await _foreUp.GetTimes(courseId, date);
        }

        return await _teeQuest.GetTimes(courseId, date);
    }

    private async Task<IDictionary<string, Coordinates>> GetZipCoordinates(HashSet<string> zips)
    {
        async Task<KeyValuePair<string, Coordinates?>> DecodeZip(string zip)
        {
            var coordinates = await _openStreets.Geocode(zip);
            return KeyValuePair.Create(zip, coordinates);
        }

        var geoTasks = zips.Select(DecodeZip).ToList();
        await Task.WhenAll(geoTasks);
        return geoTasks.Where(gt => gt.Result.Value is not null)
            .ToDictionary(kv => kv.Result.Key, kv => kv.Result.Value!);
    }
}