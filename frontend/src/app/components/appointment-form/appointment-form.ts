import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Api } from '../../services/api';
import { Appointment } from '../../models/appointment.model';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-appointment-form',
  imports: [CommonModule, FormsModule],
  templateUrl: './appointment-form.html',
  styleUrl: './appointment-form.css',
})
export class AppointmentForm implements OnInit {
  patients: Patient[] = [];
  appointment: Appointment = { patientId: '', patientName: '', date: '', reason: '', status: 'Scheduled' };

  constructor(private api: Api, private router: Router) {}

  ngOnInit() {
    this.api.getPatients().subscribe((data) => (this.patients = data));
  }

  onPatientChange() {
    const p = this.patients.find((p) => p.id === this.appointment.patientId);
    this.appointment.patientName = p ? p.name : '';
  }

  save() {
    this.api.createAppointment(this.appointment).subscribe(() => {
      this.router.navigate(['/appointments']);
    });
  }
}
