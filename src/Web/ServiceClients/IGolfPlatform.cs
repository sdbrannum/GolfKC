using Web.Dtos;

namespace Web.ServiceClients;

public interface IGolfPlatform
{
    Task<IEnumerable<Course>> GetCourses();
    Task<IEnumerable<TeeTime>> GetTimes(string courseId, DateOnly date);
}