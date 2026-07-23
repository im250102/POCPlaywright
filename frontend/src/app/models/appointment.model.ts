export interface Appointment {
  id?: string;
  patientId: string;
  patientName: string;
  date: string;
  reason: string;
  status: string;
}
