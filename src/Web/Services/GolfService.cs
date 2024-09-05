using System.Globalization;
using Web.Dtos;
using Web.Integrations;

namespace Web.Services;

public class GolfService : IGolfService
{
    private readonly IGolfBack _golfBack;
    private readonly IGolfNow _golfNow;
    private readonly ITeeQuest _teeQuest;
    private readonly IChronoGolf _chronoGolf;
    private readonly IForeUp _foreUp;
    private readonly IVermontSystems _vermontSystems;
    private readonly IClubProphet _clubProphet;
    private readonly ITeeItUp _teeItUp;
    private readonly IOpenStreets _openStreets;
    private uint RADIUS = 30;
    private const double LATITUDE = 39.01470395078284;
    private const double LONGITUDE = -94.69324882561136;
    private readonly HashSet<string> _states = new() {"MO", "KS"};

    public GolfService(
        IGolfBack golfBack,
        IGolfNow golfNow,
        ITeeQuest teeQuest,
        IChronoGolf chronoGolf,
        IForeUp foreUp,
        IVermontSystems vermontSystems,
        IClubProphet clubProphet,
        ITeeItUp teeItUp,
        IOpenStreets openStreets)
    {
        _golfBack = golfBack;
        _golfNow = golfNow;
        _teeQuest = teeQuest;
        _chronoGolf = chronoGolf;
        _foreUp = foreUp;
        _vermontSystems = vermontSystems;
        _clubProphet = clubProphet;
        _teeItUp = teeItUp;
        _openStreets = openStreets;
    }

    public async Task<IEnumerable<Course>> GetCourses(DateOnly date)
    {
        var formattedDate = date.ToString("MMMM dd yyyy", new CultureInfo("en-us"));
        var golfNowCourses = _golfNow.GetCourses();
        var foreUpCourses = _foreUp.GetCourses();
        var golfBackCourses = _golfBack.GetCourses();
        var teeQuestCourses = _teeQuest.GetCourses();
        var chronoCourses = _chronoGolf.GetCourses();
        var vermontSystemCourses = _vermontSystems.GetCourses();
        var clubProphetCourses = _clubProphet.GetCourses();
        var teeItUpCourses = _teeItUp.GetCourses();

        return golfNowCourses.Concat(foreUpCourses)
            .Concat(golfBackCourses)
            .Concat(teeQuestCourses)
            .Concat(chronoCourses)
            .Concat(vermontSystemCourses)
            .Concat(clubProphetCourses)
            .Concat(teeItUpCourses);
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTeeTimes(Source source, string courseId, DateOnly date)
    {
        return source switch
        {
            Source.GolfBack => await _golfBack.GetTimes(courseId, date),
            Source.GolfNow => await _golfNow.GetTimes(courseId, date),
            Source.Chrono => await _chronoGolf.GetTimes(courseId, date),
            Source.ForeUp => await _foreUp.GetTimes(courseId, date),
            Source.TeeQuest => await _teeQuest.GetTimes(courseId, date),
            Source.VermontSystems => await _vermontSystems.GetTimes(courseId, date),
            Source.ClubProphet => await _clubProphet.GetTimes(courseId, date),
            Source.TeeItUp => await _teeItUp.GetTimes(courseId, date),
            _ => Result<IEnumerable<TeeTime>>.Fail($"Unknown source: {source}")
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