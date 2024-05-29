using FastCache.Extensions;
using Web.Dtos;
using Web.Integrations.Dtos;

namespace Web.Mappers;

public static class CourseMapper
{
    public static Course Map(GolfNowFacility gnf)
    {
        return new Course
        {
            Id = gnf.Id.ToString(),
            Name = gnf.Name,
            Photo = new Uri(gnf.ThumbnailImagePath),
            Source = Source.GolfNow,
            Address = new Address
            {
                City = gnf.Address.City,
                State = gnf.Address.StateProvinceCode,
            }
        };
    }

    public static Course Map(GreatLifeCourse glc)
    {
        return new Course
        {
            Id = glc.Id,
            Name = glc.Name,
            // Photo = new Uri(glc.PhotoUrl),
            Source = Source.GreatLife,
            // Range = null,
            Address = new Address
            {
                City = glc.Address.City,
                State = glc.Address.State,
            }
        };
    }

    public static Course Map(TeeQuestCourse course)
    {
        return new Course
        {
            Id = course.Id,
            Name = course.Name,
            Source = Source.TeeQuest,
            Address = new Address
            {
                City = course.Address.City,
                State = course.Address.State
            }
        };
    }
    
    public static Course Map(ChronoCourse course)
    {
        return new Course
        {
            Id = course.Id,
            Name = course.Name,
            Source = Source.Chrono,
            Address = new Address
            {
                City = course.Address.City,
                State = course.Address.State
            }
        };
    }
}