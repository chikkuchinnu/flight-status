# Flight Status Lookup - Specification

## Overview
This document defines the data models, interface contracts, and business logic for the Flight Status lookup feature on the SkyRoute platform.

## Data Models

### Request Model
```
FlightStatusRequest
├── flightNumber: string (required, non-empty)
└── date: string (required, format: yyyy-MM-dd)
```

### Unified Status Enum
```
FlightStatus
├── OnTime       (departure/arrival within 15 minutes of schedule)
├── Delayed      (departure/arrival >15 minutes beyond schedule)
├── Cancelled    (flight will not operate)
├── Diverted     (landed at different airport)
└── Unknown      (no usable status from providers)
```

### Provider Response Models

#### AeroTrack (Full Detail)
```
AeroTrackResponse
├── flightNumber: string
├── date: string
├── status: "On Time" | "Delayed" | "Cancelled" | "Diverted" | null
├── scheduledDepartureUtc: DateTime
├── actualDepartureUtc: DateTime (nullable)
├── scheduledArrivalUtc: DateTime
├── actualArrivalUtc: DateTime (nullable)
├── departureTerminal: string (nullable)
├── departureGate: string (nullable)
├── delayReason: string (nullable)
└── lastUpdatedUtc: DateTime
```

#### QuickFlight (Minimal Detail)
```
QuickFlightResponse
├── flightNumber: string
├── date: string
├── status: "ON_TIME" | "DELAYED" | "CANCELLED" | "DIVERTED" | null
├── scheduledDepartureUtc: DateTime
├── scheduledArrivalUtc: DateTime
└── lastUpdatedUtc: DateTime
```

### Unified Result Model
```
FlightStatusResult
├── flightNumber: string
├── date: string
├── status: FlightStatus (enum)
├── normalizedStatus: string (human-readable)
├── scheduledDepartureUtc: DateTime
├── actualDepartureUtc: DateTime (nullable)
├── scheduledArrivalUtc: DateTime
├── actualArrivalUtc: DateTime (nullable)
├── departureTerminal: string (nullable)
├── departureGate: string (nullable)
├── delayReason: string (nullable)
├── lastUpdatedUtc: DateTime
├── sourceProvider: string ("AeroTrack" | "QuickFlight")
└── message: string (error or info message)
```

## Service Interfaces

### IFlightStatusProvider
```csharp
public interface IFlightStatusProvider
{
    string ProviderName { get; }
    Task<ProviderResponse?> GetFlightStatusAsync(string flightNumber, string date);
}

public abstract class ProviderResponse
{
    public string FlightNumber { get; set; }
    public string Date { get; set; }
    public string? Status { get; set; }
    public DateTime ScheduledDepartureUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public DateTime ScheduledArrivalUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
```

### IFlightStatusService
```csharp
public interface IFlightStatusService
{
    Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, string date);
    FlightStatus NormalizeStatus(string? providerStatus, string providerName);
    FlightStatusResult MergeResponses(ProviderResponse? aeroTrack, ProviderResponse? quickFlight);
}
```

## Business Logic Rules

### Status Normalization
Maps provider-specific status strings to unified enum:

**AeroTrack Mapping:**
- "On Time" → OnTime
- "Delayed" → Delayed
- "Cancelled" → Cancelled
- "Diverted" → Diverted
- null or unknown → Unknown

**QuickFlight Mapping:**
- "ON_TIME" → OnTime
- "DELAYED" → Delayed
- "CANCELLED" → Cancelled
- "DIVERTED" → Diverted
- null or unknown → Unknown

### Merge Rules (Priority Order)
1. **Both providers respond:** Use response with later `lastUpdatedUtc`
2. **One provider responds:** Use that response
3. **Neither responds:** Return status=Unknown with message "No data available from any provider"

## API Contract

### Endpoint
```
GET /flights/status?flightNumber={code}&date={yyyy-MM-dd}
```

### Success Response (200)
```json
{
  "flightNumber": "SK123",
  "date": "2024-12-25",
  "status": "OnTime",
  "normalizedStatus": "On Time",
  "scheduledDepartureUtc": "2024-12-25T10:00:00Z",
  "actualDepartureUtc": "2024-12-25T10:12:00Z",
  "scheduledArrivalUtc": "2024-12-25T14:30:00Z",
  "actualArrivalUtc": null,
  "departureTerminal": "T1",
  "departureGate": "A5",
  "delayReason": null,
  "lastUpdatedUtc": "2024-12-25T10:20:00Z",
  "sourceProvider": "AeroTrack",
  "message": "Status retrieved successfully"
}
```

## Provider Stub Specifications

Stubs use hash-based determinism: hash(flightNumber + date) % scenarios determines scenario.

### Test Flight Numbers
- `SK100-SK105` → On Time
- `BA200-BA205` → Delayed
- `LH300-LH305` → Cancelled
- `AF400-AF405` → Diverted
- `EZY500-EZY505` → No Status

---

**Document Version:** 1.0
