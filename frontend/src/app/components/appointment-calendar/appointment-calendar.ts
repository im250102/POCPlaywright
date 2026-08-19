import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { CalendarAppointment } from '../../models/appointment.model';

@Component({
  selector: 'app-appointment-calendar',
  imports: [CommonModule, RouterLink],
  templateUrl: './appointment-calendar.html',
  styleUrl: './appointment-calendar.css',
})
export class AppointmentCalendar implements OnInit {
  appointments: CalendarAppointment[] = [];
  current = new Date();

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getCalendarAppointments().subscribe({
      next: (data) => {
        this.appointments = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading calendar:', err),
    });
  }

  get monthLabel(): string {
    const label = this.current.toLocaleDateString('es-ES', { month: 'long', year: 'numeric' });
    return label.charAt(0).toUpperCase() + label.slice(1);
  }

  get weeks(): Date[][] {
    const year = this.current.getFullYear();
    const month = this.current.getMonth();
    const first = new Date(year, month, 1);
    const startOffset = (first.getDay() + 6) % 7;
    const start = new Date(year, month, 1 - startOffset);

    const weeks: Date[][] = [];
    for (let w = 0; w < 6; w++) {
      const week: Date[] = [];
      for (let d = 0; d < 7; d++) {
        week.push(new Date(start.getFullYear(), start.getMonth(), start.getDate() + w * 7 + d));
      }
      weeks.push(week);
    }
    return weeks;
  }

  appointmentsFor(day: Date): CalendarAppointment[] {
    return this.appointments.filter((a) => {
      const date = new Date(a.date);
      return (
        date.getFullYear() === day.getFullYear() &&
        date.getMonth() === day.getMonth() &&
        date.getDate() === day.getDate()
      );
    });
  }

  isCurrentMonth(day: Date): boolean {
    return day.getMonth() === this.current.getMonth() && day.getFullYear() === this.current.getFullYear();
  }

  isToday(day: Date): boolean {
    const today = new Date();
    return (
      day.getFullYear() === today.getFullYear() &&
      day.getMonth() === today.getMonth() &&
      day.getDate() === today.getDate()
    );
  }

  prevMonth() {
    this.current = new Date(this.current.getFullYear(), this.current.getMonth() - 1, 1);
    this.cdr.detectChanges();
  }

  nextMonth() {
    this.current = new Date(this.current.getFullYear(), this.current.getMonth() + 1, 1);
    this.cdr.detectChanges();
  }
}