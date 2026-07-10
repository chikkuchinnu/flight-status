import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-search-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './search-form.component.html',
  styleUrl: './search-form.component.scss'
})
export class SearchFormComponent {
  @Output() search = new EventEmitter<{ flightNumber: string; date: string }>();
  @Output() reset = new EventEmitter<void>();

  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      flightNumber: ['', [Validators.required, Validators.minLength(2)]],
      date: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.form.valid) {
      this.search.emit({
        flightNumber: this.form.get('flightNumber')?.value || '',
        date: this.form.get('date')?.value || ''
      });
    }
  }

  onReset() {
    this.form.reset();
    this.reset.emit();
  }

  get flightNumber() {
    return this.form.get('flightNumber');
  }

  get date() {
    return this.form.get('date');
  }
}
