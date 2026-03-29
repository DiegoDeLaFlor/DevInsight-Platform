# DevInsight AI Engine

Servicio FastAPI responsable de transformar resultados de análisis estático en insights accionables.

## Responsabilidades

- Recibir resultados estructurados de análisis (issues por archivo).
- Generar explicaciones y sugerencias de mejora.
- Asignar nivel de riesgo por tipo de hallazgo.

## Ejecutar en local

```bash
python -m venv .venv
source .venv/Scripts/activate
pip install -r requirements.txt
uvicorn app.main:app --host 0.0.0.0 --port 8000
```

## Verificación rápida

```bash
py -m compileall app
```

## Endpoints

- `GET /health`
- `POST /insights`

## Notas

- `OPENAI_API_KEY` se configura por variable de entorno.
- Si no hay integración LLM disponible, el servicio puede operar con respuestas fallback definidas por implementación.
