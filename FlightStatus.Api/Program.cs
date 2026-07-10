using FlightStatus.Api.Models;
using FlightStatus.Api.Services;
using FlightStatus.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddScoped<IFlightStatusService, FlightStatusService>();
builder.Services.AddScoped<IFlightStatusProvider, AeroTrackStub>();
builder.Services.AddScoped<IFlightStatusProvider, QuickFlightStub>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

// GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}
app.MapGet("/flights/status", async Task<IResult> (string? flightNumber, string? date, IFlightStatusService service) =>
{
    if (string.IsNullOrWhiteSpace(flightNumber) || string.IsNullOrWhiteSpace(date))
    {
        return Results.BadRequest(new
        {
            status = "Unknown",
            message = "flightNumber and date are required"
        });
    }

    if (!IsValidDate(date))
    {
        return Results.BadRequest(new
        {
            status = "Unknown",
            message = "date must be in yyyy-MM-dd format"
        });
    }

    try
    {
        var result = await service.GetFlightStatusAsync(flightNumber.ToUpper(), date);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.StatusCode(500);
    }
});

app.Run("http://localhost:5000");

static bool IsValidDate(string date)
{
    return DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
}
