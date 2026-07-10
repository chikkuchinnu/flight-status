using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public class QuickFlightStub : IFlightStatusProvider
{
    public string ProviderName => "QuickFlight";

    // Dummy data list with all flight statuses
    private static readonly List<QuickFlightResponse> _dummyFlights = new()
    {
        new QuickFlightResponse
        {
            FlightNumber = "AA100",
            Date = "2024-01-15",
            Status = "ON_TIME",
            ScheduledDepartureUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc),
            ActualDepartureUtc = new DateTime(2024, 1, 15, 10, 2, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2024, 1, 15, 14, 30, 0, DateTimeKind.Utc),
            ActualArrivalUtc = new DateTime(2024, 1, 15, 14, 28, 0, DateTimeKind.Utc),
            LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-10)
        },
        new QuickFlightResponse
        {
            FlightNumber = "BA200",
            Date = "2024-01-16",
            Status = "DELAYED",
            ScheduledDepartureUtc = new DateTime(2024, 1, 16, 8, 30, 0, DateTimeKind.Utc),
            ActualDepartureUtc = null,
            ScheduledArrivalUtc = new DateTime(2024, 1, 16, 12, 0, 0, DateTimeKind.Utc),
            ActualArrivalUtc = null,
            LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-5)
        },
        new QuickFlightResponse
        {
            FlightNumber = "UA300",
            Date = "2024-01-17",
            Status = "CANCELLED",
            ScheduledDepartureUtc = new DateTime(2024, 1, 17, 15, 45, 0, DateTimeKind.Utc),
            ActualDepartureUtc = null,
            ScheduledArrivalUtc = new DateTime(2024, 1, 17, 19, 15, 0, DateTimeKind.Utc),
            ActualArrivalUtc = null,
            LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-30)
        },
        new QuickFlightResponse
        {
            FlightNumber = "DL400",
            Date = "2024-01-18",
            Status = "DIVERTED",
            ScheduledDepartureUtc = new DateTime(2024, 1, 18, 11, 20, 0, DateTimeKind.Utc),
            ActualDepartureUtc = new DateTime(2024, 1, 18, 11, 18, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2024, 1, 18, 15, 50, 0, DateTimeKind.Utc),
            ActualArrivalUtc = null,
            LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-15)
        },
        new QuickFlightResponse
        {
            FlightNumber = "LH500",
            Date = "2024-01-19",
            Status = null,
            ScheduledDepartureUtc = new DateTime(2024, 1, 19, 6, 0, 0, DateTimeKind.Utc),
            ActualDepartureUtc = null,
            ScheduledArrivalUtc = new DateTime(2024, 1, 19, 10, 30, 0, DateTimeKind.Utc),
            ActualArrivalUtc = null,
            LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-45)
        }
    };

    public Task<ProviderResponse?> GetFlightStatusAsync(string flightNumber, string date)
    {
        // Try to find exact match in dummy data
        var flight = _dummyFlights.FirstOrDefault(f => 
            f.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase) && 
            f.Date == date);

        // Return null if no data found
        return Task.FromResult<ProviderResponse?>(flight);
    }
}
