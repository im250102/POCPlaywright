import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { Doctor, MEDICAL_SPECIALTIES } from '../../models/doctor.model';

@Component({
  selector: 'app-doctor-form',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './doctor-form.html',
  styleUrl: './doctor-form.css',
})
export class DoctorForm implements OnInit {
  doctor: Doctor = { name: '', lastName: '', phone: '', specialty: '' };
  specialties = MEDICAL_SPECIALTIES;
  error = '';
  isEdit = false;
  doctorId = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private api: Api,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    this.doctorId = this.route.snapshot.paramMap.get('id') ?? '';
    this.isEdit = this.doctorId !== '';

    if (this.isEdit) {
      this.api.getDoctor(this.doctorId).subscribe({
        next: (doctor) => {
          this.doctor = { ...doctor };
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Error loading doctor:', err),
      });
    }
  }

  save() {
    this.error = '';
    const operation = this.isEdit
      ? this.api.updateDoctor(this.doctorId, this.doctor)
      : this.api.createDoctor(this.doctor);

    operation.subscribe({
      next: () => this.router.navigate(['/doctors']),
      error: (err) => {
        this.error = err.error?.error || 'Error al guardar el doctor';
      },
    });
  }

  cancel() {
    this.router.navigate(['/doctors']);
  }
}