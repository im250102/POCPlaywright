# Tech Stack

## Backend

| Componente | Tecnología | Versión |
|------------|-----------|---------|
| Framework | ASP.NET Core | 10.0 |
| Lenguaje | C# | 13 |
| Base de datos | MongoDB | 8+ |
| Driver MongoDB | MongoDB.Driver | 3.10.0 |
| API Documentation | Swagger (Swashbuckle.AspNetCore) | 10.2.3 |
| Entorno | .NET SDK | 10.0 |

## Frontend

| Componente | Tecnología | Versión |
|------------|-----------|---------|
| Framework | Angular | 22 |
| Lenguaje | TypeScript | 6.0 |
| Target ES | ES2022 |
| Build | @angular/build | Application Builder |
| Estilos | CSS (custom properties) |
| Formateo | Prettier | 3.8.1 |

## Testing & Automatización

| Componente | Tecnología | Versión |
|------------|-----------|---------|
| E2E Testing | Playwright Test | 1.61.1 |
| Browser Automation | Playwright | 1.61.1 |
| MCP SDK | @modelcontextprotocol/sdk | 1.29.0 |
| Navegador | Chromium | headless / headed |

## Infraestructura

| Componente | Detalle |
|------------|---------|
| Backend URL | http://localhost:5091 |
| Frontend URL | http://localhost:4200 |
| MongoDB | mongodb://127.0.0.1:27017 |
| Base de datos | POCPlaywright |
| Colecciones | Patients, Appointments, MedicalReports |

## Stack visual

```
Playwright (E2E + MCP Server)
       |
   Angular 22 (SPA)
       |
ASP.NET Core 10 (REST API)
       |
   MongoDB (local)
```
r
