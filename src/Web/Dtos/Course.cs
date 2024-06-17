namespace Web.Dtos;

public sealed class Course
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Uri? Photo { get; init; }
    public Uri Uri { get; init; } = default!;
    public Source Source { get; init; }
    public Address Address { get; init; } = default!;
}

public enum Source
{
    GolfNow = 1,
    GreatLife = 2,
    TeeQuest = 3,
    Chrono = 4,
    ForeUp = 5,
    VermontSystems = 6
}