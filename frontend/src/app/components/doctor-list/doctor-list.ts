import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { Doctor, specialtyLabel } from '../../models/doctor.model';

@Component({
  selector: 'app-doctor-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './doctor-list.html',
  styleUrl: './doctor-list.css',
})
export class DoctorList implements OnInit {
  doctors: Doctor[] = [];

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading doctors:', err),
    });
  }

  deleteDoctor(id: string) {
    this.api.deleteDoctor(id).subscribe({
      next: () => {
        this.doctors = this.doctors.filter((d) => d.id !== id);
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error deleting doctor:', err),
    });
  }

  specialtyName(value: string): string {
    return specialtyLabel(value);
  }
}