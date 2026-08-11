# Guía de Despliegue — POCPlaywright

## Stack: Render (backend) + Vercel (frontend) + MongoDB Atlas

---

## 1. MongoDB Atlas (base de datos gratis)

1. Ve a https://www.mongodb.com/atlas y crea una cuenta gratuita
2. Crea un **cluster M0 (Shared, free tier, 512 MB)**
3. Una vez creado, ve a **Database Access** → Add New User:
   - Username: `pocplaywright`
   - Password: genera una segura
   - Built-in Role: `Read and write to any database`
4. Ve a **Network Access** → Add IP Address:
   - `0.0.0.0/0` (Allow from anywhere — para Render)
5. Ve a **Databases** → **Connect** → **Drivers**
   - Copia el connection string del tipo:
     `mongodb+srv://pocplaywright:<password>@cluster0.xxxxx.mongodb.net/POCPlaywright?retryWrites=true&w=majority`

---

## 2. Render (backend .NET)

### Opción A — Desde el Dashboard (recomendado)

1. Ve a https://dashboard.render.com/ → **New +** → **Web Service**
2. Conecta tu repositorio de GitHub
3. Configura:
   - **Name**: `pocplaywright-backend`
   - **Runtime**: `Docker`
   - **Build Command**: *(se lee del Dockerfile)*
   - **Start Command**: *(se lee del Dockerfile)*
   - **Plan**: **Free**
4. En **Environment Variables** añade:

| Variable | Valor |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__MongoDb` | `mongodb+srv://pocplaywright:<password>@cluster0.xxxxx.mongodb.net/POCPlaywright?retryWrites=true&w=majority` |
| `DatabaseName` | `POCPlaywright` |
| `Jwt__Key` | `UnaClaveSuperSeguraDeAlMenos32Caracteres!2026` |
| `Jwt__Issuer` | `POCPlaywright` |
| `Jwt__Audience` | `POCPlaywright` |
| `Notifications__Provider` | `telegram` |
| `Notifications__TelegramBotToken` | Token del bot de @BotFather (p. ej. `8762820966:AA...`) |
| `Notifications__TelegramChatId` | Chat ID numérico obtenido con `getUpdates` (p. ej. `2032361136`) |

5. Haz clic en **Create Web Service**

Render tardará ~3 min en construir y desplegar. Obtendrás una URL como:
`https://pocplaywright-backend.onrender.com`

### Opción B — Usando render.yaml (Infrastructure as Code)

Si prefieres conectar el repo directamente con el archivo `render.yaml` ya incluido:

1. En Render dashboard → **New +** → **Blueprint**
2. Conecta tu repo — Render detectará automáticamente el `render.yaml`
3. Te pedirá llenar los valores de las variables marcadas como `sync: false` (incluye `ConnectionStrings__MongoDb`, `Jwt__Key`, `Notifications__TelegramBotToken` y `Notifications__TelegramChatId`)

---

### Notas sobre las notificaciones de Telegram

- `Notifications__TelegramBotToken` y `Notifications__TelegramChatId` están marcadas como `sync: false` en `render.yaml` para **no commitear los secretos**. Render te las pedirá al crear el Blueprint.
- El envío a `api.telegram.org` funciona en Render porque corre fuera de la red corporativa (sin el proxy Zscaler). En local queda descartado por política de seguridad corporativa.
- `Notifications__WelcomeMessage` se toma de `appsettings.json` (incluido en la imagen Docker), no hace falta definirlo en Render.

---

## 3. Vercel (frontend Angular)

1. Actualiza `src/environments/environment.prod.ts` con la URL de tu backend en Render:
   ```ts
   export const environment = {
     production: true,
     apiUrl: 'https://pocplaywright-backend.onrender.com/api',
   };
   ```

2. Sube el código a GitHub

3. Ve a https://vercel.com/ → **Add New** → **Project**
   - Importa tu repositorio
   - **Framework Preset**: Angular
   - **Build Command**: `npm run build` *(por defecto)*
   - **Output Directory**: `dist/frontend/browser`
   - Vercel detectará automáticamente `vercel.json`

4. Haz clic en **Deploy**

Obtendrás una URL como: `https://pocplaywright.vercel.app`

---

## 4. Notas importantes

### Backend se duerme (Render Free Tier)
Render apaga el servicio tras **15 minutos sin actividad**. La primera solicitud tras el periodo de inactividad tarda ~30 segundos en responder (mientras despierta). Es normal.

### CORS
El backend ya tiene `AllowAnyOrigin()` configurado. No deberías tener problemas de CORS entre vercel.app y onrender.com.

### Actualizar la URL del frontend
Si cambia la URL de Render, actualiza `environment.prod.ts` y redeploy en Vercel.

### Comandos para build local de prueba
```bash
# Backend
cd backend
docker build -t pocplaywright-backend .

# Frontend
cd frontend
npm run build -- --configuration production
```
