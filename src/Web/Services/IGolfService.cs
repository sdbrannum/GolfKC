using Web.Dtos;

namespace Web.Services;

public interface IGolfService
{
    Task<IEnumerable<Course>> GetCourses(DateOnly date);
    Task<Result<IEnumerable<TeeTime>>> GetTeeTimes(Source source, string courseId, DateOnly date);
}