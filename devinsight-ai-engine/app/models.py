from typing import Literal

from pydantic import BaseModel, Field, HttpUrl


class CodeIssueModel(BaseModel):
    type: str = Field(min_length=3, max_length=100)
    line: int = Field(ge=1)
    severity: Literal["low", "medium", "high", "critical"]
    message: str = Field(min_length=5, max_length=1000)
    symbolName: str | None = Field(default=None, max_length=200)


class FileAnalysisModel(BaseModel):
    filePath: str = Field(min_length=1, max_length=1000)
    issues: list[CodeIssueModel]


class AnalysisRequestModel(BaseModel):
    repositoryId: str = Field(min_length=1, max_length=100)
    repositoryUrl: HttpUrl
    files: list[FileAnalysisModel]


class InsightModel(BaseModel):
    title: str
    explanation: str
    suggestedImprovement: str
    riskLevel: Literal["low", "medium", "high", "critical"]


class AnalysisInsightResponseModel(BaseModel):
    repositoryId: str
    insights: list[InsightModel]
