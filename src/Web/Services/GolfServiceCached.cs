using Web.Dtos;

namespace Web.Services;

public class GolfServiceCached : IGolfService
{
    private readonly IGolfService _service;

    public GolfServiceCached(IGolfService service)
    {
        _service = service;
    }
    
    public async Task<IEnumerable<Course>> GetCourses(DateOnly date)
    {
        return await FastCache.Cached.GetOrCompute(
            $"courses:{date.ToString("O")}", 
            (_) => _service.GetCourses(date),
        TimeSpan.FromMinutes(60));
    }

    public async Task<Result<IEnumerable<TeeTime>>> GetTeeTimes(Source source, string courseId, DateOnly date)
    {
        return await FastCache.Cached.GetOrCompute(
            $"{source}:{courseId}:times:{date.ToString("O")}", 
            (_) => _service.GetTeeTimes(source, courseId, date),
            TimeSpan.FromMinutes(3));
    }
}