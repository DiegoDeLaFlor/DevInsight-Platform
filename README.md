# DevInsight - Engineering Intelligence Platform

DevInsight es una plataforma SaaS orientada a inteligencia de ingeniería que analiza repositorios de GitHub, ejecuta análisis estático con AST y genera insights accionables.

## Estado del proyecto

Implementado en esta iteración:

- Backend con DDD + Clean Architecture para el bounded context EngineeringIntelligence.
- Integración OAuth de GitHub en backend (inicio, callback y listado de repositorios).
- Clonado de repositorios con soporte de sesión GitHub (repos públicos y privados).
- Analizador AST con Roslyn para reglas iniciales de C#.
- AI Engine en FastAPI con endpoint de insights.
- Frontend React con flujo funcional:
  - Solicitar análisis de repositorio.
  - Ver último análisis por repositoryId.
  - Renderizar issues e insights reales.

Pendiente en próximas iteraciones:

- Integración OAuth completa en frontend (login/callback/session lifecycle).
- Persistencia EF Core en lugar de almacenamiento en memoria.
- Reglas avanzadas de arquitectura, seguridad y métricas evolutivas.
- Soporte multi-lenguaje AST (actualmente solo C#).

## Estructura del monorepo

- devinsight-backend: API ASP.NET Core + capas DDD.
- devinsight-ai-engine: servicio FastAPI para generar insights.
- devinsight-frontend: SPA React con páginas Dashboard, Repository, Analysis y Chat (MVP).
- docker-compose.yml: orquestación de servicios.
- .env.example: plantilla de variables de entorno.

## Prerrequisitos

### Opción recomendada (Docker)

- Docker Desktop.
- Docker Compose v2.

### Opción local (sin Docker)

- .NET SDK 9.
- Python 3.12 recomendado para AI Engine.
- Git CLI.

Nota: con Python 3.14 puede fallar la instalación de pydantic-core en algunos entornos locales. Si ocurre, usa Docker o Python 3.12.

## Configuración inicial

1. Crear archivo .env en la raíz usando .env.example como base.
2. Ajustar valores reales para:

- GITHUB_CLIENT_ID
- GITHUB_CLIENT_SECRET
- OPENAI_API_KEY

## Ejecución con Docker

Desde la raíz del repositorio:

```bash
docker compose up --build
```

Servicios esperados:

- Backend: http://localhost:5000
- AI Engine: http://localhost:8000
- Frontend: http://localhost:5173

Importante: el frontend aún no está implementado, por lo que su contenedor puede fallar hasta completar ese módulo.

## Ejecución local por servicio

### Backend (.NET)

```bash
cd devinsight-backend
dotnet restore
dotnet build DevInsight.Backend.sln
dotnet run --project src/DevInsight.Api
```

Backend disponible en:

- http://localhost:5000 (si se respeta configuración de launch).
- Health: http://localhost:5000/health
- Swagger (Development): http://localhost:5000/swagger

### AI Engine (FastAPI)

```bash
cd devinsight-ai-engine
python -m venv .venv
source .venv/Scripts/activate
pip install -r requirements.txt
uvicorn app.main:app --host 0.0.0.0 --port 8000
```

AI Engine disponible en:

- http://localhost:8000
- Health: http://localhost:8000/health

## API principal (Backend)

### OAuth GitHub

- `GET /api/auth/github/login-url`
- `POST /api/auth/github/callback`
- `GET /api/auth/github/repositories?sessionId={sessionId}`

### Solicitar análisis de repositorio

`POST /api/engineering-intelligence/repositories/analyze`

Ejemplo:

```json
{
  "repositoryName": "aspnetcore",
  "repositoryUrl": "https://github.com/dotnet/aspnetcore",
  "branch": "main",
  "gitHubSessionId": "optional-session-id"
}
```

### Obtener último análisis por repositorio

`GET /api/engineering-intelligence/repositories/{repositoryId}/analysis/latest`

## Reglas AST implementadas

- LongMethod: método con más de 50 líneas.
- DeepNesting: anidación mayor a 3 niveles.
- LargeFile: archivo con más de 300 líneas.

Alcance actual del analizador:

- Solo archivos C# (`*.cs`) usando Roslyn.
- Repositorios sin C# pueden retornar 0 issues sin error.

## Seguridad y sesgo (estado actual)

Medidas aplicadas en esta iteración:

- Validación de host de repositorio para limitar a github.com.
- Validación de formato de branch para reducir riesgo de inyección en clonación.
- Manejo server-side de sesión GitHub (token no expuesto al frontend).
- Validación de state OAuth con expiración para mitigar replay.
- Contrato estructurado backend -> AI Engine para reducir ambigüedad en insights.

Riesgos abiertos para siguientes fases:

- Endurecer OAuth con almacenamiento distribuido de sesión, rotación y trazabilidad.
- Persistencia y auditoría de eventos de seguridad.
- Evaluación de sesgo de IA con dataset de validación multi-repo y multi-estilo.

## Próximos pasos recomendados

1. Implementar IdentityAccessManagement + OAuth GitHub end-to-end.
2. Agregar EF Core y migraciones para Repository, Analysis e Insight.
3. Conectar frontend al flujo OAuth completo y selección de repositorios autenticados.
4. Añadir pruebas de contrato entre Backend y AI Engine.
