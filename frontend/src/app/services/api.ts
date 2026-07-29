import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient } from '../models/patient.model';
import { Appointment } from '../models/appointment.model';
import { MedicalReport } from '../models/medical-report.model';

@Injectable({ providedIn: 'root' })
export class Api {
  private base = 'http://localhost:5091/api';

  constructor(private http: HttpClient) {}

  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.base}/patients`);
  }

  getPatient(id: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.base}/patients/${id}`);
  }

  createPatient(patient: Patient): Observable<Patient> {
    return this.http.post<Patient>(`${this.base}/patients`, patient);
  }

  deletePatient(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/patients/${id}`);
  }

  getAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.base}/appointments`);
  }

  getAppointmentsByPatient(patientId: string): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.base}/appointments/by-patient/${patientId}`);
  }

  createAppointment(appointment: Appointment): Observable<Appointment> {
    return this.http.post<Appointment>(`${this.base}/appointments`, appointment);
  }

  deleteAppointment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/appointments/${id}`);
  }

  getReportsByPatient(patientId: string): Observable<MedicalReport[]> {
    return this.http.get<MedicalReport[]>(`${this.base}/medicalreports/by-patient/${patientId}`);
  }

  getReports(): Observable<MedicalReport[]> {
    return this.http.get<MedicalReport[]>(`${this.base}/medicalreports`);
  }

  exportReportPdf(id: string): Observable<Blob> {
    return this.http.get(`${this.base}/medicalreports/export/${id}`, { responseType: 'blob' });
  }

  createReport(report: MedicalReport): Observable<MedicalReport> {
    return this.http.post<MedicalReport>(`${this.base}/medicalreports`, report);
  }

  deleteReport(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/medicalreports/${id}`);
  }
}
