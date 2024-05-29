using Web.Dtos;

namespace Web.Integrations.Dtos;

public class TeeQuestCourse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Address Address { get; init; } = default!;
}