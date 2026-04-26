using System.Text.Json.Serialization;

namespace Web.Integrations.Dtos;

public class ClubProphetToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = default!;
}