# DevInsight Backend - Iteration 1

This iteration initializes the EngineeringIntelligence bounded context with DDD and Clean Architecture.

## Implemented

- EngineeringIntelligence aggregate roots: `Repository`, `Analysis`, `Insight`
- Domain command/query and repository contracts
- Application command/query services and orchestration pipeline
- Infrastructure services:
  - GitHub clone service
  - Roslyn AST analyzer for C#
  - AI Engine HTTP integration
- REST API endpoints in `DevInsight.Api`
- Global exception middleware and health endpoint

## API Endpoints

- `POST /api/engineering-intelligence/repositories/analyze`
- `GET /api/engineering-intelligence/repositories/{repositoryId}/analysis/latest`
- `GET /health`

## Sample Request

```json
{
  "repositoryName": "aspnetcore",
  "repositoryUrl": "https://github.com/dotnet/aspnetcore",
  "branch": "main"
}
```

## Sample Analyze Response

```json
{
  "repositoryId": "68a026fc-a8b5-4de8-a4d4-d5a5e3e84f03",
  "analysisId": "8d6618ec-3ff6-4f91-a659-e865f8f76c4e"
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
