# Feature: Gestión de Informes Médicos

## Descripción
El sistema debe permitir gestionar informes médicos asociados a pacientes, incluyendo visualización, creación y eliminación.


## Criterios de Aceptación

- El usuario puede ver un listado de pacientes con acceso a sus informes
- Cada paciente en el listado debe tener un botón para ver sus informes
- La navegación hacia la sección de informes debe funcionar desde cualquier parte de la aplicación
- El usuario puede exportar los informes médicos de cada paciente a un archivo PDF

## Escenarios

### Escenario 1: Visualizar listado de pacientes para informes
**Dado** que el usuario está en la aplicación
**Cuando** navega a la sección de informes
**Entonces** ve una tabla con las columnas Nombre, Email y Teléfono
**Y** cada fila tiene un botón "Ver Informes"

### Escenario 2: Exportar informe médico a PDF
**Dado** que el usuario está visualizando los informes de un paciente
**Cuando** hace clic en "Exportar PDF" en el informe médico
**Entonces** el sistema genera y descarga un archivo PDF con los datos del informe
**Y** el PDF contiene el nombre del paciente, fecha, diagnóstico y recomendaciones

### Escenario 3: Navegación por sidebar
**Dado** que el usuario está en cualquier pantalla
**Cuando** hace clic en "Informes" en la barra lateral
**Entonces** se muestra la pantalla de listado de pacientes para informes
