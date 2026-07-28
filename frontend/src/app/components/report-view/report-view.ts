import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { MedicalReport } from '../../models/medical-report.model';

@Component({
  selector: 'app-report-view',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './report-view.html',
  styleUrl: './report-view.css',
})
export class ReportView implements OnInit {
  patientId: string = '';
  reports: MedicalReport[] = [];
  newReport: MedicalReport = { patientId: '', patientName: '', title: '', content: '', date: '', doctor: '' };
  showForm = false;

  constructor(private api: Api, private route: ActivatedRoute, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('patientId') || '';
    this.newReport.patientId = this.patientId;
    this.loadReports();
  }

  loadReports() {
    this.api.getReportsByPatient(this.patientId).subscribe({
      next: (data) => {
        this.reports = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading reports:', err),
    });
  }

  saveReport() {
    this.api.createReport(this.newReport).subscribe({
      next: () => {
        this.loadReports();
        this.showForm = false;
        this.newReport = { patientId: this.patientId, patientName: '', title: '', content: '', date: '', doctor: '' };
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error saving report:', err),
    });
  }

  deleteReport(id: string) {
    this.api.deleteReport(id).subscribe({
      next: () => {
        this.loadReports();
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error deleting report:', err),
    });
  }
}
