namespace FlightStatus.Api.Models;

public abstract class ProviderResponse
{
    public required string FlightNumber { get; set; }
    public required string Date { get; set; }
    public string? Status { get; set; }
    public required DateTime ScheduledDepartureUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public required DateTime ScheduledArrivalUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public required DateTime LastUpdatedUtc { get; set; }
}

public class AeroTrackResponse : ProviderResponse
{
    public string? DepartureTerminal { get; set; }
    public string? DepartureGate { get; set; }
    public string? DelayReason { get; set; }
}

public class QuickFlightResponse : ProviderResponse
{
}
