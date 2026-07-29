import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { Appointment } from '../../models/appointment.model';

function saveBlob(blob: Blob, fileName: string) {
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;
  a.click();
  window.URL.revokeObjectURL(url);
}

@Component({
  selector: 'app-appointment-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './appointment-list.html',
  styleUrl: './appointment-list.css',
})
export class AppointmentList implements OnInit {
  appointments: Appointment[] = [];

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getAppointments().subscribe({
      next: (data) => {
        this.appointments = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading appointments:', err),
    });
  }

  exportPdf() {
    this.api.exportAppointmentsPdf().subscribe({
      next: (blob) => {
        saveBlob(blob, 'citas_medicas.pdf');
      },
      error: (err) => console.error('Error exporting PDF:', err),
    });
  }

  deleteAppointment(id: string) {
    this.api.deleteAppointment(id).subscribe({
      next: () => {
        this.appointments = this.appointments.filter((a) => a.id !== id);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error deleting appointment:', err),
    });
  }
}
