export enum FlightStatus {
  OnTime = 'OnTime',
  Delayed = 'Delayed',
  Cancelled = 'Cancelled',
  Diverted = 'Diverted',
  Unknown = 'Unknown'
}

export interface FlightStatusResult {
  flightNumber: string;
  date: string;
  status: FlightStatus;
  normalizedStatus: string;
  scheduledDepartureUtc: string;
  actualDepartureUtc: string | null;
  scheduledArrivalUtc: string;
  actualArrivalUtc: string | null;
  departureTerminal: string | null;
  departureGate: string | null;
  delayReason: string | null;
  lastUpdatedUtc: string;
  sourceProvider: string;
  message: string;
}
