import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Api } from '../../services/api';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-patient-form',
  imports: [FormsModule],
  templateUrl: './patient-form.html',
  styleUrl: './patient-form.css',
})
export class PatientForm {
  patient: Patient = { name: '', email: '', phone: '', dateOfBirth: '', address: '' };

  constructor(private api: Api, private router: Router) {}

  save() {
    this.api.createPatient(this.patient).subscribe(() => {
      this.router.navigate(['/patients']);
    });
  }
}
