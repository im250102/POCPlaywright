# Spec-Driven Development

Este directorio contiene la especificación formal del proyecto **POCPlaywright**, siguiendo la metodología **Spec-Driven Development (SDD)**.

## Estructura

```
spec/
├── README.md              # Este archivo
├── constitution/          # Documentos fundacionales del proyecto
│   ├── README.md
│   ├── project-vision.md
│   ├── glossary.md
│   └── spec-standards.md
└── features/              # Especificaciones de funcionalidades
    ├── README.md
    ├── patients.md
    ├── appointments.md
    └── reports.md
```

## Principios SDD

1. **Las especificaciones son la fuente de verdad** — todo el código se escribe para cumplir las specs.
2. **Las specs son ejecutables** — deben poder validarse mediante tests automatizados.
3. **Lenguaje ubicuo** — usando la terminología del dominio (consultorio médico).
4. **Evolución constant** — las specs se refinan junto con el código.
