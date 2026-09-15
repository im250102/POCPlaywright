# Blog

## Descripción

Aplicacion de blog clasico donde cada usuario registrado en el sistema podra publicar post de diversa indole (texto plano, texto enriquecido, imagenes, enlaces, etc.).

## Alcance funcional

- **Registro y autenticacion de usuarios**: cada usuario podra crearse una cuenta, iniciar sesion y cerrar sesion.
- **Publicacion de post**: todo usuario autenticado podra crear un post. Los post podran ser de diferentes tipos/categorias (texto, tecnologia, personal, tutorial, etc.).
- **Listado y lectura de post**: cualquier visitante (autenticado o no) podra ver el listado de post y leer un post en detalle.
- **Edicion y eliminacion**: el autor de un post podra editar o eliminar sus propios post.
- **Comentarios (opcional)**: los usuarios autenticados podran comentar los post.

## Requisitos tecnicos

### Calidad y testing

Se han de implementar de forma obligatoria:

- **Test unitarios**: que cubran la logica principal de la aplicacion (servicios de negocio, validaciones de autenticacion, CRUD de post, reglas de permisos, etc.).
- **Test end-to-end (E2E)**: que validen los flujos completos del usuario a traves de la interfaz (registro, login, creacion de post, edicion, eliminacion, lectura y listado).

### Otros

- Los tests se ejecutaran de forma automatica en el pipeline de integracion continua.
- Cobertura minima exigida en tests unitarios: 80 % de las lineas de codigo principal.