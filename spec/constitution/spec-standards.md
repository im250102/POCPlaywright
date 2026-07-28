# Estándares para Especificaciones

## Formato

- Todos los archivos de spec usan **Markdown (.md)**
- Cada feature debe tener su propio archivo en `spec/features/`
- Los cambios a las specs requieren revisión explícita

## Estructura de una Feature Spec

Cada archivo de feature debe contener:

```markdown
# Nombre de la Feature

## Descripción
Breve descripción de la funcionalidad.

## Criterios de Aceptación
Lista de condiciones que deben cumplirse.

## Escenarios (Gherkin)
Dado / Cuando / Entonces
```

## Relación con Tests

- Cada escenario Gherkin debe tener un test E2E correspondiente en `playwright/tests/`
- El nombre del test debe coincidir con el escenario que valida
- Los tests se consideran la validación ejecutable de la spec

## Ciclo SDD

1. **Escribir spec** en `spec/features/`
2. **Escribir test** en `playwright/tests/` que falla (red)
3. **Implementar código** que pasa el test (green)
4. **Refactorizar** manteniendo los tests verdes
