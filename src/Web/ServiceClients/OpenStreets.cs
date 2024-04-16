using Web.Dtos;
using Web.ServiceClients.Dtos;

namespace Web.ServiceClients;

public interface IOpenStreets
{
    Task<Coordinates?> Geocode(string zip);
}
public class OpenStreets : IOpenStreets
{
    private readonly HttpClient _httpClient;

    public OpenStreets(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Coordinates?> Geocode(string zip)
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<OpenStreetsPlace>>($"search.php?q={zip}&countrycodes=us&format=jsonv2");
        var place = response?.FirstOrDefault();
        if (place is not null)
        {
            var x = double.TryParse(place.Latitude, out var lat);
            var y = double.TryParse(place.Longitude, out var lon);

            if (x && y)
            {
                return new Coordinates(lat, lon);
            }
        }

        return null;
    } 
}