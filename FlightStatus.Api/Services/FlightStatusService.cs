using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;

namespace FlightStatus.Api.Services;

public class FlightStatusService : IFlightStatusService
{
    private readonly IEnumerable<IFlightStatusProvider> _providers;

    public FlightStatusService(IEnumerable<IFlightStatusProvider> providers)
    {
        _providers = providers;
    }

    public async Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, string date)
    {
        var aeroTrackProvider = _providers.FirstOrDefault(p => p.ProviderName == "AeroTrack");
        var quickFlightProvider = _providers.FirstOrDefault(p => p.ProviderName == "QuickFlight");

        var aeroTrackResponse = aeroTrackProvider != null 
            ? await aeroTrackProvider.GetFlightStatusAsync(flightNumber, date) 
            : null;

        var quickFlightResponse = quickFlightProvider != null 
            ? await quickFlightProvider.GetFlightStatusAsync(flightNumber, date) 
            : null;

        return MergeResponses(aeroTrackResponse, quickFlightResponse);
    }

    public FlightStatus NormalizeStatus(string? providerStatus, string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerStatus))
            return FlightStatus.Unknown;

        return providerName.ToLower() switch
        {
            "aerotrack" => providerStatus.ToLower() switch
            {
                "on time" => FlightStatus.OnTime,
                "delayed" => FlightStatus.Delayed,
                "cancelled" => FlightStatus.Cancelled,
                "diverted" => FlightStatus.Diverted,
                _ => FlightStatus.Unknown
            },
            "quickflight" => providerStatus.ToUpper() switch
            {
                "ON_TIME" => FlightStatus.OnTime,
                "DELAYED" => FlightStatus.Delayed,
                "CANCELLED" => FlightStatus.Cancelled,
                "DIVERTED" => FlightStatus.Diverted,
                _ => FlightStatus.Unknown
            },
            _ => FlightStatus.Unknown
        };
    }

    public FlightStatusResult MergeResponses(ProviderResponse? aeroTrack, ProviderResponse? quickFlight)
    {
        // If neither provider has data
        if (aeroTrack == null && quickFlight == null)
        {
            return new FlightStatusResult
            {
                FlightNumber = "Unknown",
                Date = "Unknown",
                Status = FlightStatus.Unknown,
                NormalizedStatus = "Unknown",
                ScheduledDepartureUtc = DateTime.UtcNow,
                ScheduledArrivalUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                SourceProvider = "None",
                Message = "No data available from any provider"
            };
        }

        // Determine which response to use
        ProviderResponse selectedResponse;
        string sourceProvider;

        if (aeroTrack != null && quickFlight != null)
        {
            // Both available - use more recent
            if (aeroTrack.LastUpdatedUtc >= quickFlight.LastUpdatedUtc)
            {
                selectedResponse = aeroTrack;
                sourceProvider = "AeroTrack";
            }
            else
            {
                selectedResponse = quickFlight;
                sourceProvider = "QuickFlight";
            }
        }
        else if (aeroTrack != null)
        {
            selectedResponse = aeroTrack;
            sourceProvider = "AeroTrack";
        }
        else
        {
            selectedResponse = quickFlight!;
            sourceProvider = "QuickFlight";
        }

        var status = NormalizeStatus(selectedResponse.Status, sourceProvider);
        var normalizedStatusString = status switch
        {
            FlightStatus.OnTime => "On Time",
            FlightStatus.Delayed => "Delayed",
            FlightStatus.Cancelled => "Cancelled",
            FlightStatus.Diverted => "Diverted",
            _ => "Unknown"
        };

        var result = new FlightStatusResult
        {
            FlightNumber = selectedResponse.FlightNumber,
            Date = selectedResponse.Date,
            Status = status,
            NormalizedStatus = normalizedStatusString,
            ScheduledDepartureUtc = selectedResponse.ScheduledDepartureUtc,
            ActualDepartureUtc = selectedResponse.ActualDepartureUtc,
            ScheduledArrivalUtc = selectedResponse.ScheduledArrivalUtc,
            ActualArrivalUtc = selectedResponse.ActualArrivalUtc,
            LastUpdatedUtc = selectedResponse.LastUpdatedUtc,
            SourceProvider = sourceProvider,
            Message = "Status retrieved successfully"
        };

        // Add AeroTrack-specific fields if available
        if (selectedResponse is AeroTrackResponse aeroTrackSpecific)
        {
            result.DepartureTerminal = aeroTrackSpecific.DepartureTerminal;
            result.DepartureGate = aeroTrackSpecific.DepartureGate;
            result.DelayReason = aeroTrackSpecific.DelayReason;
        }

        return result;
    }
}
