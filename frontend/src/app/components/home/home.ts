import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  patientCount = 0;
  appointmentCount = 0;
  reportCount = 0;

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getPatients().subscribe({
      next: (data) => { this.patientCount = data.length; this.cdr.detectChanges(); },
      error: (err) => console.error('patients error:', err)
    });
    this.api.getAppointments().subscribe({
      next: (data) => { this.appointmentCount = data.length; this.cdr.detectChanges(); },
      error: (err) => console.error('appointments error:', err)
    });
    this.api.getReports().subscribe({
      next: (data) => { this.reportCount = data.length; this.cdr.detectChanges(); },
      error: (err) => console.error('reports error:', err)
    });
  }
}
