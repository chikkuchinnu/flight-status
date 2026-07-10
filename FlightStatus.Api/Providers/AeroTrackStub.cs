using FlightStatus.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace FlightStatus.Api.Providers;

public class AeroTrackStub : IFlightStatusProvider
{
    public string ProviderName => "AeroTrack";

    public Task<ProviderResponse?> GetFlightStatusAsync(string flightNumber, string date)
    {
        // Deterministic scenario selection
        var scenario = GetScenario(flightNumber, date);
        
        var baseDate = DateTime.ParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);
        var scheduledDeparture = baseDate.AddHours(10);
        var scheduledArrival = baseDate.AddHours(14).AddMinutes(30);

        AeroTrackResponse response = scenario switch
        {
            0 => new AeroTrackResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "On Time",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = scheduledDeparture.AddMinutes(10),
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = scheduledArrival.AddMinutes(8),
                DepartureTerminal = "T1",
                DepartureGate = "A5",
                DelayReason = null,
                LastUpdatedUtc = DateTime.UtcNow
            },
            1 => new AeroTrackResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "Delayed",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = scheduledDeparture.AddMinutes(45),
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = scheduledArrival.AddMinutes(50),
                DepartureTerminal = "T1",
                DepartureGate = "B2",
                DelayReason = "Weather conditions",
                LastUpdatedUtc = DateTime.UtcNow
            },
            2 => new AeroTrackResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "Cancelled",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = null,
                DepartureTerminal = "T2",
                DepartureGate = null,
                DelayReason = "Crew unavailable",
                LastUpdatedUtc = DateTime.UtcNow
            },
            3 => new AeroTrackResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "Diverted",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = scheduledDeparture.AddMinutes(-5),
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = baseDate.AddHours(15).AddMinutes(20),
                DepartureTerminal = "T1",
                DepartureGate = "C3",
                DelayReason = "Unscheduled diversion",
                LastUpdatedUtc = DateTime.UtcNow
            },
            _ => new AeroTrackResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = null,
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = scheduledDeparture.AddMinutes(8),
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = scheduledArrival.AddMinutes(5),
                DepartureTerminal = "T3",
                DepartureGate = "D1",
                DelayReason = null,
                LastUpdatedUtc = DateTime.UtcNow
            }
        };

        return Task.FromResult<ProviderResponse?>(response);
    }

    private int GetScenario(string flightNumber, string date)
    {
        var combined = flightNumber + date;
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        var hashValue = BitConverter.ToUInt32(hash, 0);
        return (int)(hashValue % 5);
    }
}
