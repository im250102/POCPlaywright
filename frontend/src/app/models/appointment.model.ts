export interface Appointment {
  id?: string;
  userId?: string;
  patientId: string;
  patientName: string;
  date: string;
  reason: string;
  status: string;
}

export interface CalendarAppointment {
  id?: string;
  userId: string;
  doctorName: string;
  patientId: string;
  patientName: string;
  date: string;
  reason: string;
  status: string;
}
