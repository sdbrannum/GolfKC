using Web.Dtos;

namespace Web.Integrations;

public interface IGolfPlatform
{
    IEnumerable<Course> GetCourses();
    Task<Result<IEnumerable<TeeTime>>> GetTimes(string courseId, DateOnly date);
}