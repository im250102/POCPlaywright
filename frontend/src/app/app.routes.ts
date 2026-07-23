import { Routes } from '@angular/router';
import { Home } from './components/home/home';
import { PatientList } from './components/patient-list/patient-list';
import { PatientForm } from './components/patient-form/patient-form';
import { AppointmentList } from './components/appointment-list/appointment-list';
import { AppointmentForm } from './components/appointment-form/appointment-form';
import { ReportList } from './components/report-list/report-list';
import { ReportView } from './components/report-view/report-view';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'patients', component: PatientList },
  { path: 'patients/new', component: PatientForm },
  { path: 'appointments', component: AppointmentList },
  { path: 'appointments/new', component: AppointmentForm },
  { path: 'reports', component: ReportList },
  { path: 'reports/:patientId', component: ReportView },
];
