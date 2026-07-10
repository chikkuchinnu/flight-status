using FlightStatus.Api.Models;
using FlightStatus.Api.Services;
using FlightStatus.Api.Providers;
using Moq;

namespace FlightStatus.Tests;

public class StatusNormalizationTests
{
    private readonly IFlightStatusService _service;

    public StatusNormalizationTests()
    {
        var providers = new IFlightStatusProvider[] { };
        _service = new FlightStatusService(providers);
    }

    [Theory]
    [InlineData("On Time", "AeroTrack", FlightStatus.OnTime)]
    [InlineData("Delayed", "AeroTrack", FlightStatus.Delayed)]
    [InlineData("Cancelled", "AeroTrack", FlightStatus.Cancelled)]
    [InlineData("Diverted", "AeroTrack", FlightStatus.Diverted)]
    [InlineData(null, "AeroTrack", FlightStatus.Unknown)]
    [InlineData("", "AeroTrack", FlightStatus.Unknown)]
    public void NormalizeStatus_AeroTrackStatuses_ReturnsCorrectEnum(string? status, string provider, FlightStatus expected)
    {
        // Act
        var result = _service.NormalizeStatus(status, provider);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ON_TIME", "QuickFlight", FlightStatus.OnTime)]
    [InlineData("DELAYED", "QuickFlight", FlightStatus.Delayed)]
    [InlineData("CANCELLED", "QuickFlight", FlightStatus.Cancelled)]
    [InlineData("DIVERTED", "QuickFlight", FlightStatus.Diverted)]
    [InlineData(null, "QuickFlight", FlightStatus.Unknown)]
    [InlineData("", "QuickFlight", FlightStatus.Unknown)]
    public void NormalizeStatus_QuickFlightStatuses_ReturnsCorrectEnum(string? status, string provider, FlightStatus expected)
    {
        // Act
        var result = _service.NormalizeStatus(status, provider);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NormalizeStatus_UnknownProvider_ReturnsUnknown()
    {
        // Act
        var result = _service.NormalizeStatus("Any Status", "UnknownProvider");

        // Assert
        Assert.Equal(FlightStatus.Unknown, result);
    }
}
