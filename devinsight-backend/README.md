# DevInsight Backend

Backend ASP.NET Core para DevInsight, siguiendo DDD + Clean Architecture en el bounded context EngineeringIntelligence.

## Implemented

- Aggregate roots de dominio: `Repository`, `Analysis`, `Insight`.
- Contratos de dominio para comandos/queries/repositorios.
- Command/Query services y pipeline de análisis en Application.
- Infraestructura:
  - Clonado de repositorios GitHub.
  - Analizador Roslyn AST para C#.
  - Integración HTTP con AI Engine.
  - OAuth GitHub + sesión server-side en memoria.
- API REST en DevInsight.Api.
- Global exception middleware, CORS local y health check.

## API Endpoints

- `GET /api/auth/github/login-url`
- `POST /api/auth/github/callback`
- `GET /api/auth/github/repositories?sessionId={sessionId}`
- `POST /api/engineering-intelligence/repositories/analyze`
- `GET /api/engineering-intelligence/repositories/{repositoryId}/analysis/latest`
- `GET /health`

## Sample Request

```json
{
  "repositoryName": "aspnetcore",
  "repositoryUrl": "https://github.com/dotnet/aspnetcore",
  "branch": "main",
  "gitHubSessionId": "optional-session-id"
}
```

Notas:

- `gitHubSessionId` es opcional para repos públicos.
- Para repos privados, se obtiene tras el flujo OAuth.

## Sample Analyze Response

```json
{
  "repositoryId": "68a026fc-a8b5-4de8-a4d4-d5a5e3e84f03",
  "analysisId": "8d6618ec-3ff6-4f91-a659-e865f8f76c4e"
}
```

## OAuth Callback Request Sample

```json
{
  "code": "github_code",
  "state": "oauth_state"
}
```

## OAuth Callback Response Sample

```json
{
  "sessionId": "2d3cf2f50cb149c69b0a4bd947f44f2a",
  "expiresAtUtc": "2026-03-29T12:00:00Z",
  "userLogin": "octocat"
}
```

## Sample Latest Analysis Response

```json
{
  "analysisId": "8d6618ec-3ff6-4f91-a659-e865f8f76c4e",
  "repositoryId": "68a026fc-a8b5-4de8-a4d4-d5a5e3e84f03",
  "startedAtUtc": "2026-03-28T18:00:00Z",
  "completedAtUtc": "2026-03-28T18:00:02Z",
  "issues": [
    {
      "type": "LongMethod",
      "line": 42,
      "severity": "medium",
      "message": "Method has 61 lines and exceeds threshold 50.",
      "symbolName": "BuildResponse"
    }
  ],
  "insights": [
    {
      "title": "LongMethod concentration detected",
      "explanation": "Detected 1 issue(s) of type LongMethod in the submitted analysis payload.",
      "suggestedImprovement": "Split methods into cohesive units and extract domain services.",
      "riskLevel": "medium"
    }
  ]
}
```

## Scope and Limitations

- El analizador actual solo procesa C# (`*.cs`).
- Persistencia de `Repository` y `Analysis` es en memoria (no EF Core aún).
- Sesiones OAuth son en memoria (válidas para entorno local/MVP).
