export interface Doctor {
  id?: string;
  name: string;
  lastName: string;
  phone: string;
  specialty: string;
}

export const MEDICAL_SPECIALTIES: { value: string; label: string }[] = [
  { value: 'medicina-general', label: 'Medicina General' },
  { value: 'cardiologia', label: 'Cardiología' },
  { value: 'dermatologia', label: 'Dermatología' },
  { value: 'endocrinologia', label: 'Endocrinología' },
  { value: 'gastroenterologia', label: 'Gastroenterología' },
  { value: 'geriatria', label: 'Geriatría' },
  { value: 'hematologia', label: 'Hematología' },
  { value: 'infectologia', label: 'Infectología' },
  { value: 'nefrologia', label: 'Nefrología' },
  { value: 'neumologia', label: 'Neumología' },
  { value: 'neurologia', label: 'Neurología' },
  { value: 'oncologia', label: 'Oncología' },
  { value: 'oftalmologia', label: 'Oftalmología' },
  { value: 'otorrinolaringologia', label: 'Otorrinolaringología' },
  { value: 'pediatria', label: 'Pediatría' },
  { value: 'psiquiatria', label: 'Psiquiatría' },
  { value: 'reumatologia', label: 'Reumatología' },
  { value: 'urologia', label: 'Urología' },
  { value: 'ginecologia', label: 'Ginecología' },
  { value: 'obstetricia', label: 'Obstetricia' },
  { value: 'traumatologia', label: 'Traumatología' },
  { value: 'anestesiologia', label: 'Anestesiología' },
  { value: 'radiologia', label: 'Radiología' },
  { value: 'medicina-interna', label: 'Medicina Interna' },
];

export function specialtyLabel(value: string): string {
  return MEDICAL_SPECIALTIES.find((s) => s.value === value)?.label ?? value;
}