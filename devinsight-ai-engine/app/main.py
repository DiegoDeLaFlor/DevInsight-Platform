import logging

from fastapi import FastAPI, HTTPException

from app.models import AnalysisInsightResponseModel, AnalysisRequestModel
from app.service import build_insights
from app.settings import settings

logger = logging.getLogger("devinsight-ai-engine")
logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(name)s %(message)s")

app = FastAPI(title="DevInsight AI Engine", version="0.1.0")


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok", "environment": settings.app_env}


@app.post("/insights", response_model=AnalysisInsightResponseModel)
def generate_insights(payload: AnalysisRequestModel) -> AnalysisInsightResponseModel:
    try:
        insights = build_insights(payload)
        return AnalysisInsightResponseModel(repositoryId=payload.repositoryId, insights=insights)
    except Exception as ex:
        logger.exception("Error while generating insights for repositoryId=%s", payload.repositoryId)
        raise HTTPException(status_code=500, detail="Failed to generate insights") from ex
