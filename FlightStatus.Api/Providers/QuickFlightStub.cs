using FlightStatus.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace FlightStatus.Api.Providers;

public class QuickFlightStub : IFlightStatusProvider
{
    public string ProviderName => "QuickFlight";

    public Task<ProviderResponse?> GetFlightStatusAsync(string flightNumber, string date)
    {
        // Deterministic scenario selection
        var scenario = GetScenario(flightNumber, date);
        
        var baseDate = DateTime.ParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);
        var scheduledDeparture = baseDate.AddHours(10);
        var scheduledArrival = baseDate.AddHours(14).AddMinutes(30);

        QuickFlightResponse response = scenario switch
        {
            0 => new QuickFlightResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "ON_TIME",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = null,
                LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-15)
            },
            1 => new QuickFlightResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "DELAYED",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = null,
                LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-20)
            },
            2 => new QuickFlightResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "CANCELLED",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = null,
                LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-30)
            },
            3 => new QuickFlightResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = "DIVERTED",
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = null,
                LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-25)
            },
            _ => new QuickFlightResponse
            {
                FlightNumber = flightNumber,
                Date = date,
                Status = null,
                ScheduledDepartureUtc = scheduledDeparture,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = scheduledArrival,
                ActualArrivalUtc = null,
                LastUpdatedUtc = DateTime.UtcNow.AddMinutes(-40)
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
