using System.Globalization;
using Web.Dtos;
using Web.Integrations;

namespace Web.Services;

public class GolfService : IGolfService
{
    private readonly IGreatLife _greatLife;
    private readonly IGolfNow _golfNow;
    private readonly ITeeQuest _teeQuest;
    private readonly IChronoGolf _chronoGolf;
    private readonly IForeUp _foreUp;
    private readonly IVermontSystems _vermontSystems;
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
        IVermontSystems vermontSystems,
        IOpenStreets openStreets)
    {
        _greatLife = greatLife;
        _golfNow = golfNow;
        _teeQuest = teeQuest;
        _chronoGolf = chronoGolf;
        _foreUp = foreUp;
        _vermontSystems = vermontSystems;
        _openStreets = openStreets;
    }

    public async Task<IEnumerable<Course>> GetCourses(DateOnly date)
    {
        var formattedDate = date.ToString("MMMM dd yyyy", new CultureInfo("en-us"));
        var golfNowCourses = _golfNow.GetCourses();
        var foreUpCourses = _foreUp.GetCourses();
        var greatLifeCourses = _greatLife.GetCourses();
        var teeQuestCourses = _teeQuest.GetCourses();
        var chronoCourses = _chronoGolf.GetCourses();
        var vermontSystemCourses = _vermontSystems.GetCourses();

        return golfNowCourses.Concat(foreUpCourses)
            .Concat(greatLifeCourses)
            .Concat(teeQuestCourses)
            .Concat(chronoCourses)
            .Concat(vermontSystemCourses);
    }

    public async Task<IEnumerable<TeeTime>> GetTeeTimes(Source source, string courseId, DateOnly date)
    {
        return source switch
        {
            Source.GreatLife => await _greatLife.GetTimes(courseId, date),
            Source.GolfNow => await _golfNow.GetTimes(courseId, date),
            Source.Chrono => await _chronoGolf.GetTimes(courseId, date),
            Source.ForeUp => await _foreUp.GetTimes(courseId, date),
            Source.TeeQuest => await _teeQuest.GetTimes(courseId, date),
            Source.VermontSystems => await _vermontSystems.GetTimes(courseId, date),
            _ => Enumerable.Empty<TeeTime>()
        };
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