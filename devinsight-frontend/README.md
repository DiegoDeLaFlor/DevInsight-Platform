# DevInsight Frontend

Frontend React + Vite para visualizar y operar el flujo de análisis de repositorios en DevInsight.

## Implementado

- Navegación principal: Dashboard, Repository, Analysis y Chat.
- Formulario para solicitar análisis al backend.
- Vista Analysis conectada al endpoint de último análisis por `repositoryId`.
- Render de issues AST e insights generados por AI Engine.

## Scripts

```bash
npm install
npm run dev
npm run build
```

## Variables de entorno

Crear `.env` local con:

```dotenv
VITE_API_BASE_URL=http://localhost:5000/api
```

Si no está definida, el frontend usa `http://localhost:5000/api` por defecto.

## Flujo funcional actual

1. Ir a Repository.
2. Enviar `repositoryName`, `repositoryUrl` y `branch`.
3. Copiar o usar el `repositoryId` retornado.
4. Ir a Analysis y consultar el último análisis.

## Pendiente

- Integrar flujo OAuth GitHub completo en UI (login/callback/sesión).
- Selector visual de repositorios autenticados.
- Chat con contexto de análisis real por repositorio.
