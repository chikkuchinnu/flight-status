namespace FlightStatus.Api.Models;

public class FlightStatusResult
{
    public string? FlightNumber { get; set; }
    public string? Date { get; set; }
    public FlightStatus Status { get; set; }
    public string? NormalizedStatus { get; set; }
    public DateTime? ScheduledDepartureUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public DateTime? ScheduledArrivalUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public string? DepartureTerminal { get; set; }
    public string? DepartureGate { get; set; }
    public string? DelayReason { get; set; }
    public DateTime? LastUpdatedUtc { get; set; }
    public string? SourceProvider { get; set; }
    public string? Message { get; set; }
}
