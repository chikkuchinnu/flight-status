# AI Prompts & Usage Documentation

## Overview
This document captures the key AI prompts used throughout the Flight Status Lookup project, the context, and critical decision points made with AI assistance.

## Analysis & Design Phase

### Prompt 1: Requirements Clarification
**Context:** Understanding the dual-provider architecture and merge strategy

**Prompt:**
```
I need to build a flight status lookup system with two stub providers:
- AeroTrack: Full detail (status, times, terminal, gate, delay reason)
- QuickFlight: Minimal (status, times only)

Both have different status vocabularies. How should I:
1. Design the normalization strategy?
2. Implement the merge logic when both providers respond?
3. Ensure graceful degradation if one provider fails?
```

**Decision:** Created a service layer abstraction with `IFlightStatusProvider` interface, allowing easy testing and provider swapping. Merge logic prioritizes by `lastUpdatedUtc` for freshness.

---

### Prompt 2: Status Normalization Pattern
**Context:** Mapping provider-specific statuses to unified enum

**Prompt:**
```
Design a status normalization function that:
- Handles AeroTrack's "On Time", "Delayed", "Cancelled", "Diverted"
- Handles QuickFlight's "ON_TIME", "DELAYED", "CANCELLED", "DIVERTED"
- Returns Unknown for null or unrecognized values
- Maintains provider context in the logic
```

**Decision:** Implemented `NormalizeStatus(providerStatus: string?, providerName: string)` with switch statements for each provider. Keeps provider name as parameter to handle different vocabularies elegantly.

---

## Implementation Phase

### Prompt 3: Deterministic Stub Implementation
**Context:** Creating realistic test data without external APIs

**Prompt:**
```
I need to create deterministic stubs that return consistent data based on flight number and date.
Requirements:
- Same input always produces same output
- No random/time-based behavior
- Cover 5 scenarios: On Time, Delayed, Cancelled, Diverted, No Status
- Should work offline

How can I use hashing to distribute test flights across scenarios?
```

**Decision:** Used SHA256 hash of `flightNumber + date` to deterministically select scenario. Maps hash value modulo 5 to scenarios. Test flights like SK100-SK105 always return same scenario.

---

### Prompt 4: Angular Component Architecture
**Context:** Designing responsive, reusable Angular components

**Prompt:**
```
Design an Angular application for flight status lookup with:
- Search form (flight number + date)
- Status result card with color coding
- Error display
- Loading spinner
- Responsive design

Use standalone components and modern Angular patterns (v17+).
```

**Decision:** 
- Created standalone components for flexibility
- Used reactive forms with validation
- Implemented state management in AppComponent
- Color coding: Green (OnTime), Amber (Delayed), Red (Cancelled/Diverted), Grey (Unknown)
- Mobile-first responsive design with CSS Grid

---

### Prompt 5: Merge Logic Implementation
**Context:** Combining responses from two providers with different timestamps

**Prompt:**
```
Implement merge logic that:
1. If both providers respond: use more recent (by lastUpdatedUtc)
2. If one responds: use that one
3. If neither responds: return Unknown with clear message
4. Always include AeroTrack fields (terminal, gate, reason) when available
```

**Decision:** Implemented `MergeResponses()` with explicit logic branches. Prefers AeroTrack data when equal timestamps due to richer information. Returns source provider name in result for transparency.

---

## Testing Phase

### Prompt 6: xUnit Test Strategy
**Context:** Comprehensive unit test coverage without mocking complexity

**Prompt:**
```
Create xUnit tests covering:
- Service merge logic (both providers, one provider, neither)
- Status normalization for both providers
- Provider determinism (same input = same output)
- Edge cases (null statuses, missing fields)

Use Moq for mocking providers.
```

**Decision:**
- `FlightStatusServiceTests`: Tests merge scenarios with mocked providers
- `StatusNormalizationTests`: Parameterized tests for all status mappings
- `MergeLogicTests`: Tests all merge branches and field enrichment
- `ProviderStubTests`: Verifies determinism and required fields

---

## Frontend Integration

### Prompt 7: API Communication in Angular
**Context:** Connecting frontend to .NET backend

**Prompt:**
```
How should I handle API calls in Angular to communicate with the .NET backend?
- Use fetch API or HttpClient?
- How to handle CORS?
- Error handling and user feedback?
```

**Decision:**
- Used fetch API (simpler for this POC, no extra service layer needed)
- Enabled CORS in .NET backend for localhost:3000
- Implemented loading, error, and success states
- Display clear error messages to users

---

## Key Architectural Decisions

| Decision | Rationale |
|----------|----------|
| Minimal API (.NET) | Lightweight, no boilerplate, perfect for POC |
| Standalone Angular Components | Modern pattern, no NgModule complexity |
| Deterministic Stubs | Reproducible testing, no external dependencies |
| Service Layer Abstraction | Easy to swap providers, testable with DI |
| Hash-based Scenario Selection | Distributed test coverage, deterministic behavior |
| CORS Enabled | Frontend-backend communication on localhost |
| Reactive Forms | Type-safe validation in Angular |
| Color-Coded Status | Intuitive UX, immediate visual feedback |

---

## AI Tool Usage Insights

**What Worked Well:**
- AI helped clarify architectural patterns (service layer, DI, abstraction)
- Code generation for boilerplate (getters, constructors, response models)
- Component templates and styling suggestions
- Test structure and assertion patterns
- Error handling and edge case discovery

**What Required Human Judgment:**
- Merge strategy priority (freshness vs. detail richness)
- Determinism mechanism (hash vs. lookup table)
- UI/UX design and color schemes
- Test coverage decisions and assertion logic
- API contract design and response structure

**Where AI Added Most Value:**
- Generating repetitive service methods
- Creating consistent component templates
- Producing comprehensive test cases
- Styling across responsive breakpoints
- Documentation and type definitions

---

## Lessons Learned

1. **Abstraction First:** Designing the `IFlightStatusProvider` interface early made implementation and testing much smoother.

2. **Determinism Over Realism:** Hash-based stubs provide reproducible test data. Consider test scenario distribution carefully.

3. **Merge Strategy Matters:** Simple rule (latest timestamp wins) proved effective. More complex heuristics would add little value for POC.

4. **Standalone Components Scale:** Angular standalone components are cleaner than traditional modules for small apps like this.

5. **CORS Simplicity:** Enabling CORS for localhost made development friction-free. In production, be more restrictive.

---

**Document Version:** 1.0  
**Last Updated:** 2024
