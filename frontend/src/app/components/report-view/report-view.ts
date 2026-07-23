import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { MedicalReport } from '../../models/medical-report.model';

@Component({
  selector: 'app-report-view',
  imports: [CommonModule, FormsModule],
  templateUrl: './report-view.html',
  styleUrl: './report-view.css',
})
export class ReportView implements OnInit {
  patientId: string = '';
  reports: MedicalReport[] = [];
  newReport: MedicalReport = { patientId: '', patientName: '', title: '', content: '', date: '', doctor: '' };
  showForm = false;

  constructor(private api: Api, private route: ActivatedRoute) {}

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('patientId') || '';
    this.newReport.patientId = this.patientId;
    this.loadReports();
  }

  loadReports() {
    this.api.getReportsByPatient(this.patientId).subscribe((data) => (this.reports = data));
  }

  saveReport() {
    this.api.createReport(this.newReport).subscribe(() => {
      this.loadReports();
      this.showForm = false;
      this.newReport = { patientId: this.patientId, patientName: '', title: '', content: '', date: '', doctor: '' };
    });
  }

  deleteReport(id: string) {
    this.api.deleteReport(id).subscribe(() => this.loadReports());
  }
}
