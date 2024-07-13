using System.Text.Json.Serialization;

namespace Web.Integrations.Dtos;

public class ClubProphetOptions
{
    public IEnumerable<ClubProphetCourseOptions> CourseOptions { get; init; } = Enumerable.Empty<ClubProphetCourseOptions>();
    [JsonPropertyName("webSiteId")]
    public string WebsiteId { get; init; } = default!;
}

public class ClubProphetCourseOptions
{
    public int CourseId { get; init; }
    public string CourseName { get; init; } = default!;
    [JsonPropertyName("courseGUID")]
    public string CourseGuid { get; init; } = default!;
    [JsonPropertyName("webSiteId")]
    public string WebsiteId { get; init; } = default!;
}




