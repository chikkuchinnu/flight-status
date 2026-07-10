using FlightStatus.Api.Models;

namespace FlightStatus.Api.Services;

public interface IFlightStatusService
{
    Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, string date);
    FlightStatus NormalizeStatus(string? providerStatus, string providerName);
    FlightStatusResult MergeResponses(ProviderResponse? aeroTrack, ProviderResponse? quickFlight);
}
