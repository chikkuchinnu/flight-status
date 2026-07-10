namespace FlightStatus.Api.Models;

public class FlightStatusResult
{
    public required string FlightNumber { get; set; }
    public required string Date { get; set; }
    public required FlightStatus Status { get; set; }
    public required string NormalizedStatus { get; set; }
    public required DateTime ScheduledDepartureUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public required DateTime ScheduledArrivalUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public string? DepartureTerminal { get; set; }
    public string? DepartureGate { get; set; }
    public string? DelayReason { get; set; }
    public required DateTime LastUpdatedUtc { get; set; }
    public required string SourceProvider { get; set; }
    public required string Message { get; set; }
}
