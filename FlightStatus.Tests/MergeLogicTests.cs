using FlightStatus.Api.Models;
using FlightStatus.Api.Services;
using FlightStatus.Api.Providers;

namespace FlightStatus.Tests;

public class MergeLogicTests
{
    private readonly IFlightStatusService _service;

    public MergeLogicTests()
    {
        var providers = new IFlightStatusProvider[] { };
        _service = new FlightStatusService(providers);
    }

    [Fact]
    public void MergeResponses_BothNull_ReturnsUnknown()
    {
        // Act
        var result = _service.MergeResponses(null, null);

        // Assert
        Assert.Equal(FlightStatus.Unknown, result.Status);
        Assert.Equal("None", result.SourceProvider);
    }

    [Fact]
    public void MergeResponses_AeroTrackNewer_ReturnsAeroTrack()
    {
        // Arrange
        var baseDate = DateTime.ParseExact("2024-12-25", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);
        var olderTime = DateTime.UtcNow.AddHours(-2);
        var newerTime = DateTime.UtcNow;

        var aeroTrack = new AeroTrackResponse
        {
            FlightNumber = "SK100",
            Date = "2024-12-25",
            Status = "On Time",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            DepartureTerminal = "T1",
            DepartureGate = "A5",
            LastUpdatedUtc = newerTime
        };

        var quickFlight = new QuickFlightResponse
        {
            FlightNumber = "SK100",
            Date = "2024-12-25",
            Status = "ON_TIME",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            LastUpdatedUtc = olderTime
        };

        // Act
        var result = _service.MergeResponses(aeroTrack, quickFlight);

        // Assert
        Assert.Equal("AeroTrack", result.SourceProvider);
        Assert.Equal("T1", result.DepartureTerminal);
        Assert.Equal(newerTime, result.LastUpdatedUtc);
    }

    [Fact]
    public void MergeResponses_QuickFlightNewer_ReturnsQuickFlight()
    {
        // Arrange
        var baseDate = DateTime.ParseExact("2024-12-25", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);
        var olderTime = DateTime.UtcNow.AddHours(-2);
        var newerTime = DateTime.UtcNow;

        var aeroTrack = new AeroTrackResponse
        {
            FlightNumber = "BA200",
            Date = "2024-12-25",
            Status = "Delayed",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            LastUpdatedUtc = olderTime
        };

        var quickFlight = new QuickFlightResponse
        {
            FlightNumber = "BA200",
            Date = "2024-12-25",
            Status = "DELAYED",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            LastUpdatedUtc = newerTime
        };

        // Act
        var result = _service.MergeResponses(aeroTrack, quickFlight);

        // Assert
        Assert.Equal("QuickFlight", result.SourceProvider);
        Assert.Equal(newerTime, result.LastUpdatedUtc);
    }

    [Fact]
    public void MergeResponses_AeroTrackOnly_ReturnsAeroTrack()
    {
        // Arrange
        var baseDate = DateTime.ParseExact("2024-12-25", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);

        var aeroTrack = new AeroTrackResponse
        {
            FlightNumber = "LH300",
            Date = "2024-12-25",
            Status = "Cancelled",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            DepartureTerminal = "T2",
            DepartureGate = null,
            DelayReason = "Crew unavailable",
            LastUpdatedUtc = DateTime.UtcNow
        };

        // Act
        var result = _service.MergeResponses(aeroTrack, null);

        // Assert
        Assert.Equal(FlightStatus.Cancelled, result.Status);
        Assert.Equal("AeroTrack", result.SourceProvider);
        Assert.Equal("Crew unavailable", result.DelayReason);
    }

    [Fact]
    public void MergeResponses_QuickFlightOnly_ReturnsQuickFlight()
    {
        // Arrange
        var baseDate = DateTime.ParseExact("2024-12-25", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);

        var quickFlight = new QuickFlightResponse
        {
            FlightNumber = "AF400",
            Date = "2024-12-25",
            Status = "DIVERTED",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            LastUpdatedUtc = DateTime.UtcNow
        };

        // Act
        var result = _service.MergeResponses(null, quickFlight);

        // Assert
        Assert.Equal(FlightStatus.Diverted, result.Status);
        Assert.Equal("QuickFlight", result.SourceProvider);
    }

    [Fact]
    public void MergeResponses_IncludesAeroTrackFields()
    {
        // Arrange
        var baseDate = DateTime.ParseExact("2024-12-25", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeUniversal);

        var aeroTrack = new AeroTrackResponse
        {
            FlightNumber = "SK100",
            Date = "2024-12-25",
            Status = "On Time",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            DepartureTerminal = "T1",
            DepartureGate = "C5",
            DelayReason = null,
            LastUpdatedUtc = DateTime.UtcNow
        };

        // Act
        var result = _service.MergeResponses(aeroTrack, null);

        // Assert
        Assert.NotNull(result.DepartureTerminal);
        Assert.NotNull(result.DepartureGate);
        Assert.Equal("T1", result.DepartureTerminal);
        Assert.Equal("C5", result.DepartureGate);
    }
}
