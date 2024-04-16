using Web.Dtos;

namespace Web.ServiceClients.Dtos;

public class ChronoCourse
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public Address Address { get; init; } = default!;
    public Uri Uri { get; init; } = default!;
}