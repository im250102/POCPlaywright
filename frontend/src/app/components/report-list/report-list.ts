import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Api } from '../../services/api';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-report-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './report-list.html',
  styleUrl: './report-list.css',
})
export class ReportList implements OnInit {
  patients: Patient[] = [];

  constructor(private api: Api) {}

  ngOnInit() {
    this.api.getPatients().subscribe((data) => (this.patients = data));
  }
}
