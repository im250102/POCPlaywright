import { Component, OnInit } from '@angular/core';
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

  constructor(private api: Api) {}

  ngOnInit() {
    this.api.getPatients().subscribe((data) => (this.patients = data));
  }

  deletePatient(id: string) {
    this.api.deletePatient(id).subscribe(() => {
      this.patients = this.patients.filter((p) => p.id !== id);
    });
  }
}
