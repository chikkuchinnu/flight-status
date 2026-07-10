# Flight Status Lookup - SkyRoute Platform

A full-stack flight status lookup application featuring stub integrations with two flight data providers, unified status normalization, and a responsive frontend UI.

## Features

✅ **Dual Provider Integration:** AeroTrack (full detail) and QuickFlight (minimal) stubs  
✅ **Smart Data Merge:** Selects most recent data when both providers respond  
✅ **Status Normalization:** Converts provider vocabularies to unified enum  
✅ **Responsive UI:** Real-time search with color-coded status display  
✅ **Comprehensive Tests:** Unit tests for core business logic  
✅ **Deterministic Stubs:** Consistent, offline test data  

## Tech Stack

- **Backend:** .NET 8 (Minimal API with C#)
- **Frontend:** React with TypeScript
- **Testing:** xUnit
- **Build:** dotnet CLI

## Prerequisites

- **.NET 8 SDK** (https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** (https://nodejs.org/)
- **npm** (comes with Node.js)

## Setup & Installation

### 1. Clone Repository
```bash
git clone https://github.com/chikkuchinnu/flight-status.git
cd flight-status
```

### 2. Backend Setup

```bash
cd FlightStatus.Api
dotnet restore
dotnet build
```

### 3. Frontend Setup

```bash
cd ../flight-status-ui
npm install
```

## Running the Application

### Terminal 1: Start Backend API
```bash
cd FlightStatus.Api
dotnet run
```

Expected: Now listening on http://localhost:5000

### Terminal 2: Start Frontend Dev Server
```bash
cd flight-status-ui
npm start
```

Browser opens to http://localhost:3000

## Running Tests

```bash
cd FlightStatus.Tests
dotnet test
```

## API Endpoints

### Get Flight Status
```
GET http://localhost:5000/flights/status?flightNumber=SK100&date=2024-12-25
```

## Test Flight Numbers

| Flight # | Scenario | Status | 
|----------|----------|--------|
| SK100-SK105 | On Time | OnTime |
| BA200-BA205 | Delayed | Delayed |
| LH300-LH305 | Cancelled | Cancelled |
| AF400-AF405 | Diverted | Diverted |
| EZY500-EZY505 | No Status | Varies |

---

**Status:** Ready for implementation
