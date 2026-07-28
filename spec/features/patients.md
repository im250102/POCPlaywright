# Feature: Gestión de Pacientes

## Descripción
El sistema debe permitir gestionar el registro de pacientes del consultorio médico, incluyendo creación, listado, edición y eliminación.

## Criterios de Aceptación

- El usuario puede ver un listado de todos los pacientes con nombre, email, teléfono y fecha de nacimiento
- El usuario puede crear un nuevo paciente con nombre, email, teléfono, fecha de nacimiento y dirección
- El usuario puede eliminar un paciente existente
- El formulario de creación debe tener todos los campos visibles y correctamente etiquetados
- La navegación hacia la sección de pacientes debe funcionar desde cualquier parte de la aplicación

## Escenarios

### Escenario 1: Visualizar listado de pacientes
**Dado** que el usuario está en la aplicación
**Cuando** navega a la sección de pacientes
**Entonces** ve una tabla con las columnas Nombre, Email, Teléfono y Fecha de Nacimiento

### Escenario 2: Crear un nuevo paciente
**Dado** que el usuario está en la sección de pacientes
**Cuando** hace clic en "Nuevo Paciente"
**Entonces** ve un formulario con campos para Nombre, Email, Teléfono, Fecha de Nacimiento y Dirección
**Y** un botón para guardar el paciente

### Escenario 3: Navegación por sidebar
**Dado** que el usuario está en cualquier pantalla
**Cuando** hace clic en "Pacientes" en la barra lateral
**Entonces** se muestra la pantalla de listado de pacientes
