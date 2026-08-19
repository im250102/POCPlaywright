import { Routes } from '@angular/router';
import { Home } from './components/home/home';
import { PatientList } from './components/patient-list/patient-list';
import { PatientForm } from './components/patient-form/patient-form';
import { AppointmentList } from './components/appointment-list/appointment-list';
import { AppointmentForm } from './components/appointment-form/appointment-form';
import { AppointmentCalendar } from './components/appointment-calendar/appointment-calendar';
import { ReportList } from './components/report-list/report-list';
import { ReportView } from './components/report-view/report-view';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { UserList } from './components/user-list/user-list';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '', component: Home, canActivate: [AuthGuard] },
  { path: 'patients', component: PatientList, canActivate: [AuthGuard] },
  { path: 'patients/new', component: PatientForm, canActivate: [AuthGuard] },
  { path: 'appointments', component: AppointmentList, canActivate: [AuthGuard] },
  { path: 'appointments/new', component: AppointmentForm, canActivate: [AuthGuard] },
  { path: 'appointments/calendar', component: AppointmentCalendar, canActivate: [AuthGuard] },
  { path: 'reports', component: ReportList, canActivate: [AuthGuard] },
  { path: 'reports/:patientId', component: ReportView, canActivate: [AuthGuard] },
  { path: 'users', component: UserList, canActivate: [AuthGuard, AdminGuard] },
];
