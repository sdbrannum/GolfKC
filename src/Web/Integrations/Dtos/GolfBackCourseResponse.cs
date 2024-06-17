using Web.Dtos;

namespace Web.Integrations.Dtos;

public sealed class GolfBackCourseResponse
{
    public GolfBackCourse Data { get; init; } = default!;
}

public sealed class GolfBackCourse
{
    public string Id { get; init; } = default!;
    public IEnumerable<string> MultiCourseIds { get; init; } = default!;
    public string Name { get; init; } = default!;

    public Address Address { get; init; } = default!;
    // public string Address1 { get; init; }
    // public string Address2 { get; init; }
    // public string City { get; init; }
    // public string State { get; init; }
    // public string ZipCode { get; init; }
    // public string PhoneNumber { get; init; }
    // public string PhotoUrl { get; init; }
}