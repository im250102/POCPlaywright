# Feature: Gestión de Informes Médicos

## Descripción
El sistema debe permitir gestionar informes médicos asociados a pacientes, incluyendo visualización, creación y eliminación.

## Criterios de Aceptación

- El usuario puede ver un listado de pacientes con acceso a sus informes
- Cada paciente en el listado debe tener un botón para ver sus informes
- La navegación hacia la sección de informes debe funcionar desde cualquier parte de la aplicación

## Escenarios

### Escenario 1: Visualizar listado de pacientes para informes
**Dado** que el usuario está en la aplicación
**Cuando** navega a la sección de informes
**Entonces** ve una tabla con las columnas Nombre, Email y Teléfono
**Y** cada fila tiene un botón "Ver Informes"

### Escenario 2: Navegación por sidebar
**Dado** que el usuario está en cualquier pantalla
**Cuando** hace clic en "Informes" en la barra lateral
**Entonces** se muestra la pantalla de listado de pacientes para informes
