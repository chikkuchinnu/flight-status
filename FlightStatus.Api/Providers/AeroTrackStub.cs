using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public class AeroTrackStub : IFlightStatusProvider
{
    public string ProviderName => "AeroTrack";

    // Dummy data list with all flight statuses
    private static readonly List<AeroTrackResponse> _dummyFlights = new()
    {
        new AeroTrackResponse
        {
            FlightNumber = "AA100",
            Date = "2024-01-15",
            Status = "On Time",
            ScheduledDepartureUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc),
            ActualDepartureUtc = new DateTime(2024, 1, 15, 10, 5, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2024, 1, 15, 14, 30, 0, DateTimeKind.Utc),
            ActualArrivalUtc = new DateTime(2024, 1, 15, 14, 35, 0, DateTimeKind.Utc),
            DepartureTerminal = "T1",
            DepartureGate = "A5",
            DelayReason = null,
            LastUpdatedUtc = DateTime.UtcNow
        },
        new AeroTrackResponse
        {
            FlightNumber = "BA200",
            Date = "2024-01-16",
            Status = "Delayed",
            ScheduledDepartureUtc = new DateTime(2024, 1, 16, 8, 30, 0, DateTimeKind.Utc),
            ActualDepartureUtc = new DateTime(2024, 1, 16, 9, 15, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2024, 1, 16, 12, 0, 0, DateTimeKind.Utc),
            ActualArrivalUtc = new DateTime(2024, 1, 16, 12, 50, 0, DateTimeKind.Utc),
            DepartureTerminal = "T2",
            DepartureGate = "B12",
            DelayReason = "Weather conditions",
            LastUpdatedUtc = DateTime.UtcNow
        },
        new AeroTrackResponse
        {
            FlightNumber = "UA300",
            Date = "2024-01-17",
            Status = "Cancelled",
            ScheduledDepartureUtc = new DateTime(2024, 1, 17, 15, 45, 0, DateTimeKind.Utc),
            ActualDepartureUtc = null,
            ScheduledArrivalUtc = new DateTime(2024, 1, 17, 19, 15, 0, DateTimeKind.Utc),
            ActualArrivalUtc = null,
            DepartureTerminal = "T3",
            DepartureGate = null,
            DelayReason = "Crew unavailable",
            LastUpdatedUtc = DateTime.UtcNow
        },
        new AeroTrackResponse
        {
            FlightNumber = "DL400",
            Date = "2024-01-18",
            Status = "Diverted",
            ScheduledDepartureUtc = new DateTime(2024, 1, 18, 11, 20, 0, DateTimeKind.Utc),
            ActualDepartureUtc = new DateTime(2024, 1, 18, 11, 15, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2024, 1, 18, 15, 50, 0, DateTimeKind.Utc),
            ActualArrivalUtc = new DateTime(2024, 1, 18, 16, 20, 0, DateTimeKind.Utc),
            DepartureTerminal = "T1",
            DepartureGate = "C8",
            DelayReason = "Emergency landing required",
            LastUpdatedUtc = DateTime.UtcNow
        },
        new AeroTrackResponse
        {
            FlightNumber = "LH500",
            Date = "2024-01-19",
            Status = null,
            ScheduledDepartureUtc = new DateTime(2024, 1, 19, 6, 0, 0, DateTimeKind.Utc),
            ActualDepartureUtc = new DateTime(2024, 1, 19, 6, 8, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2024, 1, 19, 10, 30, 0, DateTimeKind.Utc),
            ActualArrivalUtc = new DateTime(2024, 1, 19, 10, 38, 0, DateTimeKind.Utc),
            DepartureTerminal = "T4",
            DepartureGate = "D15",
            DelayReason = null,
            LastUpdatedUtc = DateTime.UtcNow
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
