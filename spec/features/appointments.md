# Feature: Gestión de Citas

## Descripción
El sistema debe permitir gestionar las citas médicas, incluyendo creación, visualización y cancelación. Al dar de alta una nueva cita, el usuario debe poder elegir el doctor con el que se realizará la consulta.

## Criterios de Aceptación

- El usuario puede ver un listado de todas las citas con paciente, doctor, fecha, motivo y estado
- El usuario puede crear una nueva cita seleccionando un paciente, un doctor, fecha/hora, motivo y estado
- Al crear una cita, el doctor seleccionado queda vinculado a la cita y se muestra tanto en el listado como en el calendario
- El usuario puede cancelar una cita existente
- El formulario de creación debe ser inválido si no se completa correctamente, incluida la selección del doctor
- La navegación hacia la sección de citas debe funcionar desde cualquier parte de la aplicación
- El usuario puede ver un calendario mensual con todas las citas existentes de todos los pacientes y de todos los médicos

## Escenarios

### Escenario 1: Visualizar listado de citas
**Dado** que el usuario está en la aplicación
**Cuando** navega a la sección de citas
**Entonces** ve una tabla con las columnas Paciente, Doctor, Fecha, Motivo y Estado

### Escenario 2: Crear una nueva cita
**Dado** que el usuario está en la sección de citas
**Cuando** hace clic en "Nueva Cita"
**Entonces** ve un formulario con un selector de paciente, un selector de doctor, un campo de fecha/hora y un área de texto para el motivo

### Escenario 3: Seleccionar doctor al crear una cita
**Dado** que el usuario está creando una nueva cita
**Cuando** abre el desplegable de doctor
**Entonces** ve la lista de doctores registrados en el sistema
**Y** tras seleccionar un doctor y guardar la cita
**Entonces** la cita queda asignada a ese doctor y aparece en el listado con el nombre del doctor

### Escenario 4: Validación del formulario de citas
**Dado** que el usuario está creando una nueva cita
**Cuando** el formulario está incompleto, incluyendo el caso de no haber seleccionado doctor
**Entonces** el botón de guardar debe estar deshabilitado

### Escenario 5: Navegación por sidebar
**Dado** que el usuario está en cualquier pantalla
**Cuando** hace clic en "Citas" en la barra lateral
**Entonces** se muestra la pantalla de listado de citas

### Escenario 6: Exportar citas a PDF
**Dado** que el usuario está en la sección de citas
**Cuando** hace clic en "Exportar PDF"
**Entonces** se descarga un archivo PDF con el listado de citas del paciente

### Escenario 7: Visualizar calendario mensual de citas
**Dado** que el usuario está en la sección de citas y existen citas ya creadas
**Cuando** hace clic en "Calendario"
**Entonces** ve un componente de calendario mensual que muestra todas las citas existentes de todos los pacientes y de todos los médicos
**Y** cada cita aparece en el día y hora correspondiente mostrando el nombre del paciente, el del doctor y el motivo de la consulta

## Requisitos técnicos

### Backend (ASP.NET Core)

#### Modelo de datos: `Appointment`

El modelo añade las propiedades `DoctorId` y `DoctorName` respecto a la versión anterior:

```csharp
public class Appointment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DoctorId { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled";
}
```

#### Patrón de arquitectura: Service + Result

Siguiendo el patrón establecido en Doctors, Blog y Users:

- **`AppointmentService`**: lógica de negocio con validaciones (obligatoriedad de paciente, doctor, fecha y motivo)
- **`AppointmentResult`**: patrón de resultado tipado
- **`AppointmentsController`**: controlador API que usa el servicio para validar la creación

#### Validaciones en AppointmentService

- Paciente es obligatorio
- Doctor es obligatorio (tanto `DoctorId` como `DoctorName`)
- Fecha y hora son obligatorias
- Motivo de la consulta es obligatorio
- Resolución del nombre del doctor para el calendario: prioriza `DoctorName` de la cita, luego el usuario que la creó, y si no existe devuelve "Sin asignar"

#### Endpoints API

| Método | Ruta | Descripción | Auth |
|---|---|---|---|
| `GET` | `/api/appointments` | Listar todas las citas | Sí |
| `GET` | `/api/appointments/{id}` | Obtener cita por ID | Sí |
| `GET` | `/api/appointments/by-patient/{patientId}` | Listar citas de un paciente | Sí |
| `GET` | `/api/appointments/calendar` | Citas con nombre de doctor resuelto | Sí |
| `POST` | `/api/appointments` | Crear cita (validando doctor obligatorio) | Sí |
| `PUT` | `/api/appointments/{id}` | Actualizar cita | Sí |
| `GET` | `/api/appointments/export/{patientId?}` | Exportar PDF | Sí |
| `DELETE` | `/api/appointments/{id}` | Eliminar cita | Sí |

### Frontend (Angular)

#### Modelo TypeScript: `Appointment`

```typescript
export interface Appointment {
  id?: string;
  userId?: string;
  patientId: string;
  patientName: string;
  doctorId: string;
  doctorName: string;
  date: string;
  reason: string;
  status: string;
}
```

#### Formulario de nueva cita

- El formulario carga la lista de doctores desde `GET /api/doctors`
- Selector `<select name="doctorId">` obligatorio junto al de paciente, fecha/hora y motivo
- Al seleccionar un doctor se rellena `doctorName` con el nombre y apellidos

#### Listado de citas

- La tabla añade la columna **Doctor** (nombre del doctor asignado)

#### Métodos en servicio `Api`

```typescript
getDoctors(): Observable<Doctor[]>
```

(ya existente; el formulario de cita lo reutiliza para montar el selector)

### Calidad y testing

#### Test unitarios del backend (xUnit + NSubstitute)

Ubicación: `backend.Tests/AppointmentServiceTests.cs`

Se deben cubrir los siguientes escenarios:

1. **Cita válida con doctor**: preparar cita con paciente, doctor, fecha y motivo devuelve resultado con código 200
2. **Sin paciente**: crear cita sin paciente devuelve error 400
3. **Sin doctor** (`DoctorId` vacío): devuelve error 400 con "El doctor es obligatorio"
4. **Sin nombre de doctor** (`DoctorName` vacío): devuelve error 400
5. **Sin fecha**: devuelve error 400
6. **Sin motivo**: devuelve error 400
7. **Normalización del doctor en calendario**: usa `DoctorName` cuando existe
8. **Fallback a usuario creador**: usa el nombre de usuario si la cita no tiene `DoctorName`
9. **Sin asignar**: devuelve "Sin asignar" si no hay doctor

#### Test end-to-end (Playwright)

Ubicación: `playwright/tests/appointments.spec.js`

Escenarios E2E:

1. **Listado**: tabla con columnas Paciente, Doctor, Fecha, Motivo y Estado
2. **Navegación al formulario**: clic en "Nueva Cita" navega a `/appointments/new`
3. **Campos del formulario**: formulario con 2 selects (paciente y doctor), input de fecha y textarea
4. **Selector de doctor**: el desplegable de doctor contiene los doctores registrados
5. **Crear cita con doctor**: seleccionar paciente y doctor, guardar y verificar que la cita aparece con el doctor en el listado
6. **Validación del formulario**: botón guardar deshabilitado hasta seleccionar paciente, doctor, fecha y motivo
7. **Navegación por sidebar**: clic en "Citas" en nav llega a `/appointments`
8. **Exportar PDF**: descarga de PDF al pulsar "Exportar PDF"
9. **Calendario**: muestra la cita con paciente, doctor y motivo en el día correspondiente

### Ficheros a crear/modificar

#### Backend (modificar)

| Fichero | Cambio |
|---|---|
| `backend/Models/Appointment.cs` | Añadir `DoctorId` y `DoctorName` |
| `backend/Controllers/AppointmentsController.cs` | Usar `AppointmentService` para validar creación y resolver el nombre del doctor en el calendario |
| `backend/Program.cs` | Registrar `AppointmentService` |

#### Backend (nuevos)

| Fichero | Descripción |
|---|---|
| `backend/Services/AppointmentService.cs` | Lógica de negocio y validaciones |
| `backend/Services/AppointmentResult.cs` | Patrón de resultado |
| `backend.Tests/AppointmentServiceTests.cs` | Test unitarios |

#### Frontend (modificar)

| Fichero | Cambio |
|---|---|
| `frontend/src/app/models/appointment.model.ts` | Añadir `doctorId` y `doctorName` |
| `frontend/src/app/components/appointment-form/appointment-form.ts` | Cargar doctores y rellenar `doctorName` |
| `frontend/src/app/components/appointment-form/appointment-form.html` | Selector de doctor obligatorio |
| `frontend/src/app/components/appointment-list/appointment-list.html` | Columna Doctor en la tabla |

#### Playwright (modificar)

| Fichero | Cambio |
|---|---|
| `playwright/tests/appointments.spec.js` | Actualizar escenarios existentes y añadir tests de selección de doctor |