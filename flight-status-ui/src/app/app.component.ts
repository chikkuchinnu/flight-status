import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchFormComponent } from './components/search-form/search-form.component';
import { StatusCardComponent } from './components/status-card/status-card.component';
import { ErrorDisplayComponent } from './components/error-display/error-display.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { FlightStatusResult } from './types/flight';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    SearchFormComponent,
    StatusCardComponent,
    ErrorDisplayComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  flightStatus: FlightStatusResult | null = null;
  loading = false;
  error: string | null = null;
  searched = false;

  onSearch(flightData: { flightNumber: string; date: string }) {
    this.loading = true;
    this.error = null;
    this.flightStatus = null;
    this.searched = true;

    const { flightNumber, date } = flightData;
    const url = `http://localhost:5000/flights/status?flightNumber=${encodeURIComponent(flightNumber)}&date=${encodeURIComponent(date)}`;

    fetch(url)
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        this.flightStatus = data;
        this.loading = false;
      })
      .catch(error => {
        this.error = `Failed to fetch flight status: ${error.message}`;
        this.loading = false;
      });
  }

  onReset() {
    this.flightStatus = null;
    this.error = null;
    this.searched = false;
  }
}
