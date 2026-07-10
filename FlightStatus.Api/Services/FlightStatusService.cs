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

    public FlightStatus.Api.Models.FlightStatus NormalizeStatus(string? providerStatus, string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerStatus))
            return FlightStatus.Api.Models.FlightStatus.Unknown;

        return providerName.ToLower() switch
        {
            "aerotrack" => providerStatus.ToLower() switch
            {
                "on time" => FlightStatus.Api.Models.FlightStatus.OnTime,
                "delayed" => FlightStatus.Api.Models.FlightStatus.Delayed,
                "cancelled" => FlightStatus.Api.Models.FlightStatus.Cancelled,
                "diverted" => FlightStatus.Api.Models.FlightStatus.Diverted,
                _ => FlightStatus.Api.Models.FlightStatus.Unknown
            },
            "quickflight" => providerStatus.ToUpper() switch
            {
                "ON_TIME" => FlightStatus.Api.Models.FlightStatus.OnTime,
                "DELAYED" => FlightStatus.Api.Models.FlightStatus.Delayed,
                "CANCELLED" => FlightStatus.Api.Models.FlightStatus.Cancelled,
                "DIVERTED" => FlightStatus.Api.Models.FlightStatus.Diverted,
                _ => FlightStatus.Api.Models.FlightStatus.Unknown
            },
            _ => FlightStatus.Api.Models.FlightStatus.Unknown
        };
    }

    public FlightStatusResult MergeResponses(ProviderResponse? aeroTrack, ProviderResponse? quickFlight)
    {
        // If neither provider has data, return empty result
        if (aeroTrack == null && quickFlight == null)
        {
            return new FlightStatusResult
            {
                FlightNumber = null,
                Date = null,
                Status = FlightStatus.Api.Models.FlightStatus.Unknown,
                NormalizedStatus = null,
                ScheduledDepartureUtc = null,
                ActualDepartureUtc = null,
                ScheduledArrivalUtc = null,
                ActualArrivalUtc = null,
                DepartureTerminal = null,
                DepartureGate = null,
                DelayReason = null,
                LastUpdatedUtc = null,
                SourceProvider = null,
                Message = "No flight data found"
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
            FlightStatus.Api.Models.FlightStatus.OnTime => "On Time",
            FlightStatus.Api.Models.FlightStatus.Delayed => "Delayed",
            FlightStatus.Api.Models.FlightStatus.Cancelled => "Cancelled",
            FlightStatus.Api.Models.FlightStatus.Diverted => "Diverted",
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
