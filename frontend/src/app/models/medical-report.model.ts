export interface MedicalReport {
  id?: string;
  patientId: string;
  patientName: string;
  title: string;
  content: string;
  date: string;
  doctor: string;
}
