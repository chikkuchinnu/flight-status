import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlightStatusResult, FlightStatus } from '../../types/flight';

@Component({
  selector: 'app-status-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './status-card.component.html',
  styleUrl: './status-card.component.scss'
})
export class StatusCardComponent {
  @Input() result: FlightStatusResult | null = null;

  getStatusClass(status: FlightStatus): string {
    switch (status) {
      case FlightStatus.OnTime:
        return 'status-ontime';
      case FlightStatus.Delayed:
        return 'status-delayed';
      case FlightStatus.Cancelled:
        return 'status-cancelled';
      case FlightStatus.Diverted:
        return 'status-diverted';
      default:
        return 'status-unknown';
    }
  }

  getStatusBadgeClass(status: FlightStatus): string {
    switch (status) {
      case FlightStatus.OnTime:
        return 'badge-success';
      case FlightStatus.Delayed:
        return 'badge-warning';
      case FlightStatus.Cancelled:
        return 'badge-danger';
      case FlightStatus.Diverted:
        return 'badge-danger';
      default:
        return 'badge-secondary';
    }
  }

  formatDateTime(dateTimeString: string): string {
    try {
      const date = new Date(dateTimeString);
      return date.toLocaleString();
    } catch {
      return dateTimeString;
    }
  }
}
