from collections import Counter

from app.models import AnalysisRequestModel, InsightModel


def build_insights(payload: AnalysisRequestModel) -> list[InsightModel]:
    issues = [issue for file_result in payload.files for issue in file_result.issues]
    if not issues:
        return [
            InsightModel(
                title="No major issues detected",
                explanation="The current analysis did not find threshold violations for the configured rules.",
                suggestedImprovement="Keep quality gates and run periodic trend analysis to catch regressions early.",
                riskLevel="low",
            )
        ]

    counts = Counter(issue.type for issue in issues)
    insights: list[InsightModel] = []

    for issue_type, count in counts.items():
        suggested = {
            "LongMethod": "Split methods into cohesive units and extract domain services.",
            "DeepNesting": "Use guard clauses and early returns to flatten control flow.",
            "LargeFile": "Refactor responsibilities into smaller modules and enforce file-size gates.",
        }.get(issue_type, "Apply targeted refactoring and enforce static quality rules in CI.")

        risk_level = "high" if issue_type == "LargeFile" else "medium"
        insights.append(
            InsightModel(
                title=f"{issue_type} concentration detected",
                explanation=f"Detected {count} issue(s) of type {issue_type} in the submitted analysis payload.",
                suggestedImprovement=suggested,
                riskLevel=risk_level,
            )
        )

    return insights
