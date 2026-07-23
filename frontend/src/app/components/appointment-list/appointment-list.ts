import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { Appointment } from '../../models/appointment.model';

@Component({
  selector: 'app-appointment-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './appointment-list.html',
  styleUrl: './appointment-list.css',
})
export class AppointmentList implements OnInit {
  appointments: Appointment[] = [];

  constructor(private api: Api) {}

  ngOnInit() {
    this.api.getAppointments().subscribe((data) => (this.appointments = data));
  }

  deleteAppointment(id: string) {
    this.api.deleteAppointment(id).subscribe(() => {
      this.appointments = this.appointments.filter((a) => a.id !== id);
    });
  }
}
