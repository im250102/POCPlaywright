import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient } from '../models/patient.model';
import { Appointment } from '../models/appointment.model';
import { MedicalReport } from '../models/medical-report.model';
import { UserItem, UserAccessItem } from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class Api {
  private base = environment.apiUrl;

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

  exportAppointmentsPdf(patientId?: string): Observable<Blob> {
    const url = patientId
      ? `${this.base}/appointments/export/${patientId}`
      : `${this.base}/appointments/export`;
    return this.http.get(url, { responseType: 'blob' });
  }

  createReport(report: MedicalReport): Observable<MedicalReport> {
    return this.http.post<MedicalReport>(`${this.base}/medicalreports`, report);
  }

  deleteReport(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/medicalreports/${id}`);
  }

  getUsers(): Observable<UserItem[]> {
    return this.http.get<UserItem[]>(`${this.base}/users`);
  }

  getUserAccesses(userId: string): Observable<UserAccessItem[]> {
    return this.http.get<UserAccessItem[]>(`${this.base}/users/${userId}/accesses`);
  }

  updateUserRole(userId: string, role: string): Observable<void> {
    return this.http.put<void>(`${this.base}/users/${userId}/role`, { role });
  }
}
