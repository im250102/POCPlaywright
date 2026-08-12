# Feature: Roles de Usuario y Administración de Usuarios

## Descripción
El sistema debe disponer de diferentes tipos de usuarios. Existirá un usuario administrador con el perfil de **Director de la clínica** y el resto de usuarios serán los **médicos** que trabajan en la clínica. Cuando el perfil Director accede a la web, podrá visualizar una nueva pantalla, accesible junto al nombre del usuario (sección "Administración usuarios"), donde se mostrará un listado de los usuarios de la web junto con el listado de accesos a la web de cada uno.

## Criterios de Aceptación

- El sistema distingue dos roles: **Admin** (Director de la clínica) y **Medico**
- El primer usuario registrado en el sistema recibe el rol `Admin` (Director de la clínica)
- El resto de usuarios registrados reciben el rol `Medico`
- El rol del usuario se muestra en el listado de administración
- Cuando el Director inicia sesión, ve junto a su nombre de usuario un enlace a la sección **"Administración usuarios"**
- Un Médico no ve el enlace de administración de usuarios
- La pantalla de administración muestra el listado de todos los usuarios con nombre, email, rol, fecha de registro y último acceso
- La pantalla de administración permite ver el listado de accesos (inicios de sesión) de cada usuario
- Cada inicio de sesión de un usuario queda registrado en el sistema
- Solo el rol Admin puede acceder al listado de usuarios; el resto recibe 403 Forbidden
- El Director puede cambiar el rol de un usuario (convertir a un médico en Director y viceversa)
- La API rechaza peticiones sin token con 401 Unauthorized

## Modelo de Datos

### User (modificación)
```
Id: ObjectId
Name: string (nombre completo)
Email: string (único)
PasswordHash: string
Role: string ("Admin" | "Medico")
CreatedAt: DateTime
LastLoginAt: DateTime (último inicio de sesión)
```

### UserAccess (nuevo)
```
Id: ObjectId
UserId: string (ObjectId del usuario)
LoginAt: DateTime (fecha y hora del acceso)
```

## Endpoints API

### POST /api/auth/login (modificación)
Además de validar credenciales y devolver el token, actualiza `LastLoginAt` del usuario y registra un `UserAccess`.

**Response (200):**
```json
{
  "id": "...",
  "name": "Dra. María López",
  "email": "maria@consultorio.com",
  "role": "Admin",
  "token": "jwt_token..."
}
```

### GET /api/users
Listado de todos los usuarios con su último acceso.

**Headers:** `Authorization: Bearer <token>`

**Response (200):**
```json
[
  {
    "id": "...",
    "name": "Dra. María López",
    "email": "maria@consultorio.com",
    "role": "Admin",
    "createdAt": "2026-08-01T09:00:00Z",
    "lastLoginAt": "2026-08-12T08:30:00Z"
  }
]
```

**Errores:**
- 401 si no hay token válido
- 403 si el rol del token no es `Admin`

### GET /api/users/{id}/accesses
Listado de accesos a la web de un usuario, ordenados del más reciente al más antiguo.

**Headers:** `Authorization: Bearer <token>`

**Response (200):**
```json
[
  {
    "id": "...",
    "userId": "...",
    "loginAt": "2026-08-12T08:30:00Z"
  }
]
```

**Errores:**
- 401 si no hay token válido
- 403 si el rol del token no es `Admin`

### PUT /api/users/{id}/role
Cambiar el rol de un usuario.

**Request:**
```json
{
  "role": "Medico"
}
```

**Response:** 204 No Content

**Errores:**
- 401 si no hay token válido
- 403 si el rol del token no es `Admin`
- 400 si el rol no es válido (`Admin` | `Medico`)
- 404 si el usuario no existe

## Frontend

### Sidebar
- Para el usuario con rol `Admin`, junto al nombre de usuario (sección del perfil), se muestra un enlace "Administración usuarios"
- El enlace navega a la ruta `/users`
- Para el rol `Medico` el enlace no se muestra

### Pantalla de Administración de Usuarios
- Ruta: `/users`
- Título de la pantalla: "Administración usuarios"
- Tabla con columnas: Nombre, Email, Rol, Fecha de registro y Último acceso
- Cada fila tiene un botón "Ver accesos" que muestra el listado de accesos a la web del usuario
- El listado de accesos muestra la fecha y hora de cada inicio de sesión
- Para cada fila, el rol se puede modificar mediante un control de cambio de rol
- La ruta `/users` está protegida: solo accesible si la sesión es Admin

## Escenarios

### Escenario 1: El primer usuario registrado es Director de la clínica
**Dado** que no existe ningún usuario en el sistema
**Cuando** se registra el primer profesional
**Entonces** su rol asignado es `Admin`
**Y** se convierte en el Director de la clínica

### Escenario 2: El Director ve el enlace de administración de usuarios
**Dado** que el Director de la clínica está autenticado
**Cuando** ve la barra lateral junto a su nombre de usuario
**Entonces** ve un enlace a la sección "Administración usuarios"

### Escenario 3: El Médico no ve el enlace de administración
**Dado** que un Médico está autenticado
**Cuando** ve la barra lateral junto a su nombre de usuario
**Entonces** no ve el enlace de "Administración usuarios"

### Escenario 4: Visualizar el listado de usuarios
**Dado** que el Director está en la sección de administración de usuarios
**Cuando** se carga la pantalla
**Entonces** ve una tabla con todos los usuarios (nombre, email, rol, fecha de registro y último acceso)

### Escenario 5: Visualizar los accesos a la web de un usuario
**Dado** que el Director está visualizando el listado de usuarios
**Cuando** hace clic en "Ver accesos" de un usuario
**Entonces** ve el listado de inicios de sesión del usuario con su fecha y hora

### Escenario 6: El Médico no puede acceder a la administración
**Dado** que un Médico está autenticado
**Cuando** intenta acceder a `/users`
**Entonces** es redirigido fuera de la sección de administración
**Y** la API responde con 403 Forbidden

### Escenario 7: El Director cambia el rol de un usuario
**Dado** que el Director está en la sección de administración de usuarios
**Cuando** cambia el rol de un Médico a Admin
**Entonces** el sistema guarda el nuevo rol
**Y** el listado refleja el cambio