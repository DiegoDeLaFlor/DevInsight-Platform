export type Severity = 'low' | 'medium' | 'high' | 'critical'

export interface CodeIssue {
  type: string
  line: number
  severity: Severity
  message: string
  symbolName?: string | null
}

export interface Insight {
  title: string
  explanation: string
  suggestedImprovement: string
  riskLevel: Severity
}

export interface AnalysisResult {
  analysisId: string
  repositoryId: string
  startedAtUtc: string
  completedAtUtc?: string | null
  issues: CodeIssue[]
  insights: Insight[]
}

export interface AnalyzeRepositoryRequest {
  repositoryName: string
  repositoryUrl: string
  branch?: string | null
}

export interface AnalyzeRepositoryResponse {
  repositoryId: string
  analysisId: string
}
