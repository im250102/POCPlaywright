# Feature: Gestión de Doctores

## Descripción

El sistema debe permitir gestionar el registro de doctores que trabajan en la clínica médica, incluyendo creación, listado, edición y eliminación. Cada doctor tiene un nombre, apellidos, teléfono y una especialidad médica seleccionada de un desplegable con las especialidades típicas de una clínica médica.

## Alcance funcional

- **Alta de doctor**: cualquier usuario autenticado podrá crear un nuevo doctor indicando nombre, apellidos, teléfono y especialidad.
- **Listado de doctores**: cualquier usuario autenticado podrá ver el listado completo de doctores con todos sus datos.
- **Edición de doctor**: el usuario autenticado podrá modificar los datos de un doctor existente.
- **Baja de doctor**: el usuario autenticado podrá eliminar un doctor del sistema.

## Especialidades médicas disponibles

El desplegable de especialidad deberá contener las siguientes opciones:

| Código | Especialidad |
|---|---|
| `medicina-general` | Medicina General |
| `cardiologia` | Cardiología |
| `dermatologia` | Dermatología |
| `endocrinologia` | Endocrinología |
| `gastroenterologia` | Gastroenterología |
| `geriatria` | Geriatría |
| `hematologia` | Hematología |
| `infectologia` | Infectología |
| `nefrologia` | Nefrología |
| `neumologia` | Neumología |
| `neurologia` | Neurología |
| `oncologia` | Oncología |
| `oftalmologia` | Oftalmología |
| `otorrinolaringologia` | Otorrinolaringología |
| `pediatria` | Pediatría |
| `psiquiatria` | Psiquiatría |
| `reumatologia` | Reumatología |
| `urologia` | Urología |
| `ginecologia` | Ginecología |
| `obstetricia` | Obstetricia |
| `traumatologia` | Traumatología |
| `anestesiologia` | Anestesiología |
| `radiologia` | Radiología |
| `medicina-interna` | Medicina Interna |

## Criterios de Aceptación

- El usuario puede ver un listado de todos los doctores con nombre, apellidos, teléfono y especialidad
- El usuario puede crear un nuevo doctor con nombre, apellidos, teléfono y especialidad (seleccionada de un desplegable)
- El usuario puede editar los datos de un doctor existente
- El usuario puede eliminar un doctor existente
- El formulario de creación/edición debe tener todos los campos visibles y correctamente etiquetados
- El desplegable de especialidad debe mostrar todas las especialidades médicas configuradas
- La navegación hacia la sección de doctores debe funcionar desde cualquier parte de la aplicación
- Todos los campos del formulario son obligatorios
- El teléfono debe seguir un formato válido

## Escenarios

### Escenario 1: Visualizar listado de doctores
**Dado** que el usuario está en la aplicación
**Cuando** navega a la sección de doctores
**Entonces** ve una tabla con las columnas Nombre, Apellidos, Teléfono y Especialidad

### Escenario 2: Crear un nuevo doctor
**Dado** que el usuario está en la sección de doctores
**Cuando** hace clic en "Nuevo Doctor"
**Entonces** ve un formulario con campos para Nombre, Apellidos, Teléfono y un desplegable de Especialidad
**Y** un botón para guardar el doctor

### Escenario 3: Seleccionar especialidad
**Dado** que el usuario está en el formulario de nuevo doctor
**Cuando** hace clic en el desplegable de especialidad
**Entonces** ve la lista completa de especialidades médicas disponibles

### Escenario 4: Editar un doctor existente
**Dado** que el usuario está en el listado de doctores
**Cuando** hace clic en "Editar" de un doctor
**Entonces** ve el formulario prellenado con los datos actuales del doctor
**Y** puede modificar cualquier campo y guardar los cambios

### Escenario 5: Eliminar un doctor
**Dado** que el usuario está en el listado de doctores
**Cuando** hace clic en "Eliminar" de un doctor
**Entonces** el doctor se elimina del listado

### Escenario 6: Navegación por sidebar
**Dado** que el usuario está en cualquier pantalla
**Cuando** hace clic en "Doctores" en la barra lateral
**Entonces** se muestra la pantalla de listado de doctores

### Escenario 7: Validación de campos obligatorios
**Dado** que el usuario está en el formulario de nuevo doctor
**Cuando** intenta guardar sin rellenar todos los campos
**Entonces** el botón de guardar está deshabilitado

## Requisitos técnicos

### Backend (ASP.NET Core)

#### Modelo de datos: `Doctor`

```csharp
public class Doctor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}
```

#### DTOs

```csharp
public class DoctorRequest
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}

public class DoctorDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}
```

#### Endpoints API

| Método | Ruta | Descripción | Auth |
|---|---|---|---|
| `GET` | `/api/doctors` | Listar todos los doctores | Sí |
| `GET` | `/api/doctors/{id}` | Obtener doctor por ID | Sí |
| `POST` | `/api/doctors` | Crear nuevo doctor | Sí |
| `PUT` | `/api/doctors/{id}` | Actualizar doctor | Sí |
| `DELETE` | `/api/doctors/{id}` | Eliminar doctor | Sí |

#### Patrón de arquitectura: Repository + Service + Result

Siguiendo el patrón establecido en Blog y Users:

- **`IDoctorRepository`**: interfaz del repositorio con operaciones CRUD
- **`MongoDoctorRepository`**: implementación MongoDB del repositorio
- **`DoctorService`**: lógica de negocio con validaciones
- **`DoctorResult` / `DoctorResult<T>`**: patrón de resultado tipado
- **`DoctorsController`**: controlador API que usa el servicio

#### Validaciones en DoctorService

- Nombre es obligatorio y no puede estar vacío
- Apellidos son obligatorios y no pueden estar vacíos
- Teléfono es obligatorio y no puede estar vacío
- Especialidad es obligatoria y debe ser una de las especialidades permitidas

#### Colección MongoDB

- Nombre de la colección: `Doctors`
- Se añade propiedad `Doctors` en `MongoDbContext`

### Frontend (Angular)

#### Modelo TypeScript: `Doctor`

```typescript
export interface Doctor {
  id?: string;
  name: string;
  lastName: string;
  phone: string;
  specialty: string;
}
```

#### Constante de especialidades

```typescript
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
```

#### Componentes Angular

| Componente | Ruta | Descripción |
|---|---|---|
| `DoctorList` | `/doctors` | Listado de doctores con tabla y botón eliminar |
| `DoctorForm` | `/doctors/new` | Formulario de creación con desplegable de especialidad |
| `DoctorForm` | `/doctors/:id/edit` | Formulario de edición (mismo componente) |

#### Rutas

```typescript
{ path: 'doctors', component: DoctorList, canActivate: [AuthGuard] },
{ path: 'doctors/new', component: DoctorForm, canActivate: [AuthGuard] },
{ path: 'doctors/:id/edit', component: DoctorForm, canActivate: [AuthGuard] },
```

#### Métodos en servicio `Api`

```typescript
getDoctors(): Observable<Doctor[]>
getDoctor(id: string): Observable<Doctor>
createDoctor(doctor: Doctor): Observable<Doctor>
updateDoctor(id: string, doctor: Doctor): Observable<Doctor>
deleteDoctor(id: string): Observable<void>
```

#### Navegación en sidebar

Añadir enlace "Doctores" en la barra lateral con icono SVG correspondiente (stethoscope o similar).

### Calidad y testing

#### Test unitarios del backend (xUnit + NSubstitute)

Ubicación: `backend.Tests/DoctorServiceTests.cs`

Se deben cubrir los siguientes escenarios:

1. **Creación válida**: crear doctor con datos válidos normaliza y guarda correctamente
2. **Nombre vacío**: crear doctor con nombre vacío devuelve error 400
3. **Apellidos vacíos**: crear doctor con apellidos vacíos devuelve error 400
4. **Teléfono vacío**: crear doctor con teléfono vacío devuelve error 400
5. **Especialidad vacía**: crear doctor sin especialidad devuelve error 400
6. **Especialidad inválida**: crear doctor con especialidad no válida devuelve error 400
7. **Obtener doctor existente**: devuelve el doctor con código 200
8. **Obtener doctor inexistente**: devuelve 404
9. **Actualizar doctor existente**: actualiza y devuelve 200
10. **Actualizar doctor inexistente**: devuelve 404
11. **Actualizar con datos inválidos**: devuelve error 400 y no actualiza
12. **Eliminar doctor existente**: elimina y devuelve 200
13. **Eliminar doctor inexistente**: devuelve 404
14. **Listar doctores**: devuelve lista de doctores
15. **Validación de especialidades**: `[Theory]` con InlineData para cada especialidad permitida y casos inválidos

#### Test end-to-end (Playwright)

Ubicación: `playwright/tests/doctors.spec.js`

Escenarios E2E:

1. **Listado vacío**: al navegar a `/doctors` se muestra tabla vacía con mensaje "No hay doctores registrados"
2. **Navegación al formulario**: clic en "Nuevo Doctor" navega a `/doctors/new`
3. **Campos del formulario**: formulario tiene 3 inputs + 1 select + botón guardar
4. **Crear doctor**: formulario completo, guardar, verificar en listado, eliminar
5. **Especialidades en desplegable**: select contiene todas las especialidades configuradas
6. **Navegación desde sidebar**: clic en "Doctores" en nav llega a `/doctors`
7. **Editar doctor**: crear doctor, editar, verificar cambios, eliminar
8. **Eliminar doctor**: crear doctor, eliminar, verificar que desaparece
9. **Validación de formulario**: botón guardar deshabilitado con campos vacíos
10. **Flujo completo CRUD**: crear -> listar -> editar -> verificar -> eliminar

### Ficheros a crear/modificar

#### Backend (nuevos)

| Fichero | Descripción |
|---|---|
| `backend/Models/Doctor.cs` | Modelo de datos |
| `backend/Controllers/DoctorsController.cs` | Controlador API |
| `backend/Services/IDoctorRepository.cs` | Interfaz del repositorio |
| `backend/Services/MongoDoctorRepository.cs` | Implementación MongoDB |
| `backend/Services/DoctorService.cs` | Lógica de negocio |
| `backend/Services/DoctorResult.cs` | Patrón de resultado |
| `backend.Tests/DoctorServiceTests.cs` | Test unitarios |

#### Backend (modificar)

| Fichero | Cambio |
|---|---|
| `backend/Models/Dtos.cs` | Añadir `DoctorRequest` y `DoctorDto` |
| `backend/Services/MongoDbContext.cs` | Añadir colección `Doctors` |
| `backend/Program.cs` | Registrar `IDoctorRepository`, `MongoDoctorRepository`, `DoctorService` |

#### Frontend (nuevos)

| Fichero | Descripción |
|---|---|
| `frontend/src/app/models/doctor.model.ts` | Interfaz `Doctor` + constante `MEDICAL_SPECIALTIES` |
| `frontend/src/app/components/doctor-list/doctor-list.ts` | Componente listado |
| `frontend/src/app/components/doctor-list/doctor-list.html` | Plantilla listado |
| `frontend/src/app/components/doctor-list/doctor-list.css` | Estilos listado |
| `frontend/src/app/components/doctor-form/doctor-form.ts` | Componente formulario (crear/editar) |
| `frontend/src/app/components/doctor-form/doctor-form.html` | Plantilla formulario |
| `frontend/src/app/components/doctor-form/doctor-form.css` | Estilos formulario |

#### Frontend (modificar)

| Fichero | Cambio |
|---|---|
| `frontend/src/app/app.routes.ts` | Añadir rutas de doctores |
| `frontend/src/app/app.html` | Añadir enlace "Doctores" en sidebar |
| `frontend/src/app/services/api.ts` | Añadir métodos CRUD de doctores |

#### Playwright (nuevos)

| Fichero | Descripción |
|---|---|
| `playwright/tests/doctors.spec.js` | Tests E2E |
