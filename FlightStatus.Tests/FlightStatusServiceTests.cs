using FlightStatus.Api.Models;
using FlightStatus.Api.Services;
using FlightStatus.Api.Providers;
using Moq;

namespace FlightStatus.Tests;

public class FlightStatusServiceTests
{
    private readonly Mock<IFlightStatusProvider> _aeroTrackMock;
    private readonly Mock<IFlightStatusProvider> _quickFlightMock;
    private readonly IFlightStatusService _service;

    public FlightStatusServiceTests()
    {
        _aeroTrackMock = new Mock<IFlightStatusProvider>();
        _aeroTrackMock.Setup(p => p.ProviderName).Returns("AeroTrack");
        
        _quickFlightMock = new Mock<IFlightStatusProvider>();
        _quickFlightMock.Setup(p => p.ProviderName).Returns("QuickFlight");

        var providers = new[] { _aeroTrackMock.Object, _quickFlightMock.Object };
        _service = new FlightStatusService(providers);
    }

    [Fact]
    public async Task GetFlightStatusAsync_BothProvidersRespond_ReturnsMoreRecentData()
    {
        // Arrange
        var flightNumber = "SK100";
        var date = "2024-12-25";
        var baseDate = DateTime.ParseExact(date, "yyyy-MM-dd", null);
        var olderTime = baseDate.AddHours(1);
        var newerTime = baseDate.AddHours(2);

        var aeroTrackResponse = new AeroTrackResponse
        {
            FlightNumber = flightNumber,
            Date = date,
            Status = "On Time",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ActualDepartureUtc = baseDate.AddHours(10).AddMinutes(5),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            ActualArrivalUtc = null,
            DepartureTerminal = "T1",
            DepartureGate = "A5",
            DelayReason = null,
            LastUpdatedUtc = olderTime
        };

        var quickFlightResponse = new QuickFlightResponse
        {
            FlightNumber = flightNumber,
            Date = date,
            Status = "ON_TIME",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ActualDepartureUtc = null,
            ScheduledArrivalUtc = baseDate.AddHours(14),
            ActualArrivalUtc = null,
            LastUpdatedUtc = newerTime
        };

        _aeroTrackMock.Setup(p => p.GetFlightStatusAsync(flightNumber, date))
            .ReturnsAsync(aeroTrackResponse);
        _quickFlightMock.Setup(p => p.GetFlightStatusAsync(flightNumber, date))
            .ReturnsAsync(quickFlightResponse);

        // Act
        var result = await _service.GetFlightStatusAsync(flightNumber, date);

        // Assert
        Assert.Equal(FlightStatus.OnTime, result.Status);
        Assert.Equal("QuickFlight", result.SourceProvider);
        Assert.Equal(newerTime, result.LastUpdatedUtc);
    }

    [Fact]
    public async Task GetFlightStatusAsync_OnlyAeroTrackResponds_ReturnsAeroTrackData()
    {
        // Arrange
        var flightNumber = "BA200";
        var date = "2024-12-25";
        var baseDate = DateTime.ParseExact(date, "yyyy-MM-dd", null);

        var aeroTrackResponse = new AeroTrackResponse
        {
            FlightNumber = flightNumber,
            Date = date,
            Status = "Delayed",
            ScheduledDepartureUtc = baseDate.AddHours(10),
            ActualDepartureUtc = baseDate.AddHours(10).AddMinutes(45),
            ScheduledArrivalUtc = baseDate.AddHours(14),
            ActualArrivalUtc = null,
            DepartureTerminal = "T1",
            DepartureGate = "B2",
            DelayReason = "Weather",
            LastUpdatedUtc = DateTime.UtcNow
        };

        _aeroTrackMock.Setup(p => p.GetFlightStatusAsync(flightNumber, date))
            .ReturnsAsync(aeroTrackResponse);
        _quickFlightMock.Setup(p => p.GetFlightStatusAsync(flightNumber, date))
            .ReturnsAsync((ProviderResponse?)null);

        // Act
        var result = await _service.GetFlightStatusAsync(flightNumber, date);

        // Assert
        Assert.Equal(FlightStatus.Delayed, result.Status);
        Assert.Equal("AeroTrack", result.SourceProvider);
        Assert.Equal("Weather", result.DelayReason);
        Assert.Equal("T1", result.DepartureTerminal);
    }

    [Fact]
    public async Task GetFlightStatusAsync_NeitherProviderResponds_ReturnsUnknown()
    {
        // Arrange
        var flightNumber = "XX999";
        var date = "2024-12-25";

        _aeroTrackMock.Setup(p => p.GetFlightStatusAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((ProviderResponse?)null);
        _quickFlightMock.Setup(p => p.GetFlightStatusAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((ProviderResponse?)null);

        // Act
        var result = await _service.GetFlightStatusAsync(flightNumber, date);

        // Assert
        Assert.Equal(FlightStatus.Unknown, result.Status);
        Assert.Equal("None", result.SourceProvider);
        Assert.Contains("No data available", result.Message);
    }
}
