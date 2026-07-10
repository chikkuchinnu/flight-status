using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public interface IFlightStatusProvider
{
    string ProviderName { get; }
    Task<ProviderResponse?> GetFlightStatusAsync(string flightNumber, string date);
}
