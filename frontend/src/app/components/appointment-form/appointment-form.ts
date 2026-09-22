import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { AuthService } from '../../services/auth.service';
import { Appointment } from '../../models/appointment.model';
import { Doctor } from '../../models/doctor.model';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-appointment-form',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './appointment-form.html',
  styleUrl: './appointment-form.css',
})
export class AppointmentForm implements OnInit {
  patients: Patient[] = [];
  doctors: Doctor[] = [];
  appointment: Appointment = {
    patientId: '',
    patientName: '',
    doctorId: '',
    doctorName: '',
    date: '',
    reason: '',
    status: 'Scheduled',
  };

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
    this.api.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading doctors:', err),
    });
  }

  onPatientChange() {
    const p = this.patients.find((p) => p.id === this.appointment.patientId);
    this.appointment.patientName = p ? p.name : '';
  }

  onDoctorChange() {
    const d = this.doctors.find((d) => d.id === this.appointment.doctorId);
    this.appointment.doctorName = d ? `${d.name} ${d.lastName}` : '';
  }

  save() {
    this.api.createAppointment(this.appointment).subscribe({
      next: () => this.router.navigate(['/appointments']),
      error: (err) => console.error('Error creating appointment:', err),
    });
  }
}
