using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Views.Home;

public class Index : PageModel
{
    public void OnGet()
    {
        ViewData["env"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }
}