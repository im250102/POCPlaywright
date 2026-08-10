# Feature: Gestión de Pacientes

## Descripción
El sistema debe permitir gestionar el registro de pacientes del consultorio médico, incluyendo creación, listado, edición y eliminación. Además, al dar de alta a un nuevo paciente, el sistema debe notificarle por WhatsApp o Telegram indicando que ha sido dado de alta en el sistema.

## Criterios de Aceptación

- El usuario puede ver un listado de todos los pacientes con nombre, email, teléfono y fecha de nacimiento
- El usuario puede crear un nuevo paciente con nombre, email, teléfono, fecha de nacimiento y dirección
- El usuario puede eliminar un paciente existente
- El formulario de creación debe tener todos los campos visibles y correctamente etiquetados
- La navegación hacia la sección de pacientes debe funcionar desde cualquier parte de la aplicación
- Tras crear un nuevo paciente, el sistema debe enviar un mensaje de bienvenida por WhatsApp o Telegram al número de teléfono indicado en el formulario
- El sistema debe exponer una interfaz de comunicación con WhatsApp y/o Telegram para poder interactuar con los pacientes

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

### Escenario 4: Notificación de alta por WhatsApp o Telegram
**Dado** que el usuario ha rellenado el formulario de nuevo paciente con nombre, email, teléfono, fecha de nacimiento y dirección
**Cuando** guarda el paciente
**Entonces** el paciente se crea correctamente
**Y** el sistema envía un mensaje de bienvenida por WhatsApp o Telegram al número de teléfono registrado
**Y** el mensaje informa al paciente de que ha sido dado de alta en el sistema

### Escenario 5: Interfaz de comunicación con el paciente
**Dado** que el sistema requiere interactuar con los pacientes
**Cuando** un paciente es dado de alta
**Entonces** el sistema debe contar con una interfaz de comunicación (WhatsApp y/o Telegram) configurada para el envío de notificaciones
**Y** el envío fallido de la notificación no debe impedir la creación del paciente
