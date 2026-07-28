import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-patient-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './patient-list.html',
  styleUrl: './patient-list.css',
})
export class PatientList implements OnInit {
  patients: Patient[] = [];

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getPatients().subscribe({
      next: (data) => {
        this.patients = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading patients:', err),
    });
  }

  deletePatient(id: string) {
    this.api.deletePatient(id).subscribe({
      next: () => {
        this.patients = this.patients.filter((p) => p.id !== id);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error deleting patient:', err),
    });
  }
}
