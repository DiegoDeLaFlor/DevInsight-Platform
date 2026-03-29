import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useSearchParams } from 'react-router-dom'
import { apiClient } from '../../services/apiClient'
import type { AnalysisResult } from '../../types/api'

export function AnalysisPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const initialRepositoryId = searchParams.get('repositoryId') ?? ''

  const [repositoryId, setRepositoryId] = useState(initialRepositoryId)
  const [analysis, setAnalysis] = useState<AnalysisResult | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const issueCountByType = useMemo(() => {
    if (!analysis) {
      return {}
    }

    return analysis.issues.reduce<Record<string, number>>((accumulator, issue) => {
      accumulator[issue.type] = (accumulator[issue.type] ?? 0) + 1
      return accumulator
    }, {})
  }, [analysis])

  async function fetchLatestAnalysis(targetRepositoryId: string) {
    if (!targetRepositoryId.trim()) {
      setError('Debes ingresar un Repository ID valido.')
      setAnalysis(null)
      return
    }

    setIsLoading(true)
    setError(null)

    try {
      const { data } = await apiClient.get<AnalysisResult>(
        `/engineering-intelligence/repositories/${encodeURIComponent(targetRepositoryId)}/analysis/latest`,
      )

      setAnalysis(data)
    } catch {
      setAnalysis(null)
      setError('No se encontro analisis para ese Repository ID o el backend no esta disponible.')
    } finally {
      setIsLoading(false)
    }
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setSearchParams({ repositoryId })
    void fetchLatestAnalysis(repositoryId)
  }

  useEffect(() => {
    if (!initialRepositoryId) {
      return
    }

    setRepositoryId(initialRepositoryId)
    void fetchLatestAnalysis(initialRepositoryId)
  }, [initialRepositoryId])

  return (
    <section>
      <h1>Analysis Results</h1>
      <p>Consulta el ultimo analisis generado por Repository ID.</p>

      <form className="panel form" onSubmit={handleSubmit}>
        <label>
          Repository ID
          <input
            value={repositoryId}
            onChange={(event) => setRepositoryId(event.target.value)}
            placeholder="7a7c1a82-d064-413a-aa81-3386013b2b44"
            required
          />
        </label>

        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Consultando...' : 'Ver ultimo analisis'}
        </button>
      </form>

      {error && <p className="error">{error}</p>}

      {analysis && (
        <div className="panel">
          <h2>Resumen</h2>
          <p>Analysis ID: {analysis.analysisId}</p>
          <p>Repository ID: {analysis.repositoryId}</p>
          <p>Started: {new Date(analysis.startedAtUtc).toLocaleString()}</p>
          <p>
            Completed:{' '}
            {analysis.completedAtUtc ? new Date(analysis.completedAtUtc).toLocaleString() : 'En progreso'}
          </p>

          <h3>Issues ({analysis.issues.length})</h3>
          {analysis.issues.length === 0 && <p>No se detectaron issues para este analisis.</p>}
          {analysis.issues.length > 0 && (
            <>
              <ul>
                {Object.entries(issueCountByType).map(([type, count]) => (
                  <li key={type}>
                    {type}: {count}
                  </li>
                ))}
              </ul>

              <div className="table-wrapper">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Type</th>
                      <th>Line</th>
                      <th>Severity</th>
                      <th>Symbol</th>
                      <th>Message</th>
                    </tr>
                  </thead>
                  <tbody>
                    {analysis.issues.map((issue, index) => (
                      <tr key={`${issue.type}-${issue.line}-${index}`}>
                        <td>{issue.type}</td>
                        <td>{issue.line}</td>
                        <td>{issue.severity}</td>
                        <td>{issue.symbolName ?? '-'}</td>
                        <td>{issue.message}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}

          <h3>AI Insights ({analysis.insights.length})</h3>
          {analysis.insights.length === 0 && <p>Aun no hay insights para este analisis.</p>}
          {analysis.insights.length > 0 && (
            <div className="insight-list">
              {analysis.insights.map((insight, index) => (
                <article key={`${insight.title}-${index}`} className="panel insight-card">
                  <h4>{insight.title}</h4>
                  <p>
                    <strong>Risk:</strong> {insight.riskLevel}
                  </p>
                  <p>{insight.explanation}</p>
                  <p>
                    <strong>Suggested improvement:</strong> {insight.suggestedImprovement}
                  </p>
                </article>
              ))}
            </div>
          )}
        </div>
      )}
    </section>
  )
}
