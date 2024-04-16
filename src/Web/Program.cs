using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Web;
using Web.Dtos;
using Web.ServiceClients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<CourseOptions>(builder.Configuration.GetSection(CourseOptions.Courses));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IGolfNow, GolfNow>(opt =>
{
    opt.BaseAddress = new Uri("https://www.golfnow.com");
});

builder.Services.AddHttpClient<IGreatLife, GreatLife>(opt =>
{
    opt.BaseAddress = new Uri("https://api.golfback.com");
});
builder.Services.AddHttpClient<IOpenStreets, OpenStreets>(opt =>
{
    opt.BaseAddress = new Uri("https://nominatim.openstreetmap.org");
});

builder.Services.AddHttpClient<IChronoGolf, ChronoGolf>(opt =>
{
    opt.BaseAddress = new Uri("https://www.chronogolf.com");
});

builder.Services.AddHttpClient<IForeUp, ForeUp>();

builder.Services.AddScoped<ITeeQuest, TeeQuest>();
builder.Services.AddScoped<IGolfService, GolfService>();
builder.Services.Decorate<IGolfService, GolfServiceCached>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/courses", async (IGolfService service) =>
        await service.GetCourses(DateOnly.FromDateTime(DateTime.Now.AddDays(3))))
    .WithName("courses")
    .WithOpenApi();

app.MapGet("{source}/tee-times/{courseId}", async (Source source, string courseId, string date, IGolfService service) =>
    {
        var d = DateOnly.Parse(date);
        return await service.GetTeeTimes(source, courseId, d);
    })
    .WithName("tee-times")
    .WithOpenApi();
app.MapRazorPages();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}