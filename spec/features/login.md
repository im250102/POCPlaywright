# Feature: Autenticación y Registro de Profesionales

## Descripción
El sistema debe permitir que múltiples profesionales médicos se registren e inicien sesión para gestionar sus propias citas, pacientes e informes. Cada profesional tendrá sus datos aislados del resto.

## Criterios de Aceptación

- Un profesional puede crear una cuenta con email y contraseña
- Un profesional puede iniciar sesión con sus credenciales
- Un profesional puede cerrar sesión
- Las sesiones expiran tras un periodo de inactividad
- Cada profesional solo ve sus propios pacientes, citas e informes
- No se permite registrar dos cuentas con el mismo email
- La contraseña se almacena de forma segura (hash + salt)
- El formulario de registro valida que la contraseña tenga mínimo 8 caracteres
- El formulario de registro requiere confirmar la contraseña
- El acceso a rutas protegidas redirige al login si no hay sesión activa
- La API rechaza peticiones sin token válido con 401 Unauthorized

## Modelo de Datos

### Profesional (User)
```
Id: ObjectId
Name: string (nombre completo)
Email: string (único)
PasswordHash: string
CreatedAt: DateTime
```

### Cambios en modelos existentes
Todas las entidades (Patient, Appointment, MedicalReport) deben incluir:
```
UserId: string (ObjectId del profesional propietario)
```

## Endpoints API

### POST /api/auth/register
Registrar un nuevo profesional.

**Request:**
```json
{
  "name": "Dra. María López",
  "email": "maria@consultorio.com",
  "password": "MiPassword123",
  "confirmPassword": "MiPassword123"
}
```

**Response (201):**
```json
{
  "id": "...",
  "name": "Dra. María López",
  "email": "maria@consultorio.com",
  "token": "jwt_token..."
}
```

**Errores:**
- 400 si el email ya está registrado
- 400 si las contraseñas no coinciden
- 400 si la contraseña es menor de 8 caracteres

### POST /api/auth/login
Iniciar sesión.

**Request:**
```json
{
  "email": "maria@consultorio.com",
  "password": "MiPassword123"
}
```

**Response (200):**
```json
{
  "id": "...",
  "name": "Dra. María López",
  "email": "maria@consultorio.com",
  "token": "jwt_token..."
}
```

**Errores:**
- 401 si el email no existe
- 401 si la contraseña es incorrecta

### Filtrado por usuario
Todos los endpoints CRUD existentes deben filtrar por `UserId` obtenido del token JWT:

- `GET /api/patients` → solo pacientes del profesional autenticado
- `GET /api/appointments` → solo citas del profesional autenticado
- `POST /api/patients` → asigna automáticamente el `UserId` del token
- etc.

## Frontend

### Pantallas

#### Login
- Ruta: `/login`
- Campo: email
- Campo: contraseña
- Botón: "Iniciar Sesión"
- Enlace: "¿No tienes cuenta? Regístrate"

#### Registro
- Ruta: `/register`
- Campo: nombre completo
- Campo: email
- Campo: contraseña
- Campo: confirmar contraseña
- Botón: "Crear Cuenta"
- Enlace: "¿Ya tienes cuenta? Inicia sesión"

#### Protección de rutas
- Las rutas `/`, `/patients`, `/appointments`, `/reports` redirigen a `/login` si no hay sesión
- El sidebar debe mostrar el nombre del profesional y botón de cerrar sesión

### Almacenamiento del token
- El token JWT se guarda en `localStorage` con clave `auth_token`
- El token se envía en cada petición HTTP como header `Authorization: Bearer <token>`

## Escenarios

### Escenario 1: Registro exitoso
**Dado** que un profesional no tiene cuenta
**Cuando** completa el formulario de registro con datos válidos
**Entonces** se crea su cuenta
**Y** se le redirige al dashboard

### Escenario 2: Registro con email duplicado
**Dado** que un email ya está registrado
**Cuando** otro profesional intenta registrarse con ese mismo email
**Entonces** se muestra un error "El email ya está registrado"

### Escenario 3: Inicio de sesión exitoso
**Dado** que un profesional tiene una cuenta
**Cuando** introduce email y contraseña correctos
**Entonces** accede al dashboard
**Y** ve sus propios pacientes y citas

### Escenario 4: Inicio de sesión con credenciales inválidas
**Dado** que un profesional tiene una cuenta
**Cuando** introduce una contraseña incorrecta
**Entonces** se muestra un error "Credenciales inválidas"

### Escenario 5: Acceso a ruta protegida sin sesión
**Dado** que un usuario no ha iniciado sesión
**Cuando** intenta acceder a `/patients`
**Entonces** es redirigido a `/login`

### Escenario 6: Aislamiento de datos entre profesionales
**Dado** que dos profesionales tienen pacientes registrados
**Cuando** cada uno inicia sesión
**Entonces** solo ven sus propios pacientes

### Escenario 7: Cierre de sesión
**Dado** que un profesional está autenticado
**Cuando** hace clic en "Cerrar Sesión"
**Entonces** el token se elimina
**Y** es redirigido al login

### Escenario 8: API rechaza petición sin token
**Dado** que no hay sesión activa
**Cuando** se hace una petición a `GET /api/patients`
**Entonces** la API responde con 401 Unauthorized

## Stack propuesto

| Componente | Tecnología |
|------------|-----------|
| Backend Auth | JWT (Microsoft.AspNetCore.Authentication.JwtBearer) |
| Hash de contraseña | BCrypt.Net o ASP.NET Core Identity PasswordHasher |
| Frontend Auth | Servicio Angular + AuthGuard (CanActivate) |
| Almacenamiento token | localStorage |
| Interceptor HTTP | Angular HTTP Interceptor para añadir token |

## Tareas de implementación

- [ ] Crear modelo `User` en MongoDB con campo único en email
- [ ] Crear `AuthController` con endpoints register/login
- [ ] Implementar JWT token generation
- [ ] Añadir `UserId` a Patient, Appointment, MedicalReport
- [ ] Modificar controladores para filtrar por `UserId` del token
- [ ] Crear `AuthService` en frontend (login, register, logout, isLoggedIn)
- [ ] Crear componentes `Login` y `Register`
- [ ] Implementar `AuthGuard` para proteger rutas
- [ ] Implementar `HTTP Interceptor` para añadir token a peticiones
- [ ] Actualizar sidebar con nombre de usuario y cerrar sesión
