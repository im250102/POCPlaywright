# Feature: Gestión de Citas

## Descripción
El sistema debe permitir gestionar las citas médicas, incluyendo creación, visualización y cancelación.

## Criterios de Aceptación

- El usuario puede ver un listado de todas las citas con paciente, fecha, motivo y estado
- El usuario puede crear una nueva cita seleccionando un paciente, fecha/hora, motivo y estado
- El usuario puede cancelar una cita existente
- El formulario de creación debe ser inválido si no se completa correctamente
- La navegación hacia la sección de citas debe funcionar desde cualquier parte de la aplicación

## Escenarios

### Escenario 1: Visualizar listado de citas
**Dado** que el usuario está en la aplicación
**Cuando** navega a la sección de citas
**Entonces** ve una tabla con las columnas Paciente, Fecha, Motivo y Estado

### Escenario 2: Crear una nueva cita
**Dado** que el usuario está en la sección de citas
**Cuando** hace clic en "Nueva Cita"
**Entonces** ve un formulario con un selector de paciente, un campo de fecha/hora y un área de texto para el motivo

### Escenario 3: Validación del formulario de citas
**Dado** que el usuario está creando una nueva cita
**Cuando** el formulario está incompleto
**Entonces** el botón de guardar debe estar deshabilitado

### Escenario 4: Navegación por sidebar
**Dado** que el usuario está en cualquier pantalla
**Cuando** hace clic en "Citas" en la barra lateral
**Entonces** se muestra la pantalla de listado de citas

### Escenario 5: Exportar citas a PDF
**Dado** que el usuario está en la sección de citas
**Cuando** hace clic en "Exportar PDF"
**Entonces** se descarga un archivo PDF con el listado de citas del paciente
