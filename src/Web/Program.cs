using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Web;
using Web.Dtos;
using Web.Integrations;
using Web.Services;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

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

builder.Services.AddHttpClient<IGolfBack, GolfBack>(opt =>
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
builder.Services.AddHttpClient<IClubProphet, ClubProphet>();

builder.Services.AddHttpClient<IForeUp, ForeUp>();
builder.Services.AddHttpClient<ITeeItUp, TeeItUp>(opts =>
{
    opts.BaseAddress = new Uri("https://phx-api-be-east-1b.kenna.io");
});

builder.Services.AddScoped<IVermontSystems, VermontSystems>();
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

app.MapGet("/courses", [ResponseCache(Duration = 300)] async (IGolfService service) =>
        await service.GetCourses(DateOnly.FromDateTime(DateTime.Now.AddDays(3))))
    .WithName("courses")
    .WithOpenApi();

app.MapGet("{source}/tee-times/{courseId}", [ResponseCache(Duration = 60)] async (Source source, string courseId, string date, IGolfService service) =>
    {
        var d = DateOnly.Parse(date);

        // loose date validations
        if (d < DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) ||
            d > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)))
        {
            return Results.BadRequest("Date must be within 14 days of today.");
        }
        
        var teeTimes = await service.GetTeeTimes(source, courseId, d);

        if (teeTimes.Failed)
        {
            return Results.Problem(teeTimes.Error);
        }
        
        return Results.Ok(teeTimes.Data);
    })
    .WithName("tee-times")
    .WithOpenApi();

app.MapRazorPages();
app.Run();