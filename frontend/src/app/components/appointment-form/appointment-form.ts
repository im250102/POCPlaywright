import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { AuthService } from '../../services/auth.service';
import { Appointment } from '../../models/appointment.model';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-appointment-form',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './appointment-form.html',
  styleUrl: './appointment-form.css',
})
export class AppointmentForm implements OnInit {
  patients: Patient[] = [];
  appointment: Appointment = { patientId: '', patientName: '', date: '', reason: '', status: 'Scheduled' };

  constructor(private api: Api, private router: Router, private cdr: ChangeDetectorRef, private auth: AuthService) {}

  ngOnInit() {
    this.appointment.userId = this.auth.user?.id ?? '';
    this.api.getPatients().subscribe({
      next: (data) => {
        this.patients = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading patients:', err),
    });
  }

  onPatientChange() {
    const p = this.patients.find((p) => p.id === this.appointment.patientId);
    this.appointment.patientName = p ? p.name : '';
  }

  save() {
    this.api.createAppointment(this.appointment).subscribe({
      next: () => this.router.navigate(['/appointments']),
      error: (err) => console.error('Error creating appointment:', err),
    });
  }
}
