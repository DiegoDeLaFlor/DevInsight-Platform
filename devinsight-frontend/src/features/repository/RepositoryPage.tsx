import { useState } from 'react'
import type { FormEvent } from 'react'
import { apiClient } from '../../services/apiClient'
import type { AnalyzeRepositoryRequest, AnalyzeRepositoryResponse } from '../../types/api'

export function RepositoryPage() {
  const [repositoryName, setRepositoryName] = useState('')
  const [repositoryUrl, setRepositoryUrl] = useState('')
  const [branch, setBranch] = useState('main')
  const [response, setResponse] = useState<AnalyzeRepositoryResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setResponse(null)
    setIsSubmitting(true)

    try {
      const payload: AnalyzeRepositoryRequest = {
        repositoryName,
        repositoryUrl,
        branch,
      }

      const { data } = await apiClient.post<AnalyzeRepositoryResponse>(
        '/engineering-intelligence/repositories/analyze',
        payload,
      )

      setResponse(data)
    } catch {
      setError('No fue posible solicitar el analisis. Verifica backend y datos del repositorio.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section>
      <h1>Repository Analysis</h1>
      <p>Solicita analisis de un repositorio GitHub desde el backend.</p>

      <form className="panel form" onSubmit={handleSubmit}>
        <label>
          Repository Name
          <input value={repositoryName} onChange={(event) => setRepositoryName(event.target.value)} required />
        </label>

        <label>
          Repository URL
          <input value={repositoryUrl} onChange={(event) => setRepositoryUrl(event.target.value)} required />
        </label>

        <label>
          Branch
          <input value={branch} onChange={(event) => setBranch(event.target.value)} />
        </label>

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Submitting...' : 'Analyze repository'}
        </button>
      </form>

      {response && (
        <div className="panel">
          <h2>Analysis Requested</h2>
          <p>Repository ID: {response.repositoryId}</p>
          <p>Analysis ID: {response.analysisId}</p>
        </div>
      )}

      {error && <p className="error">{error}</p>}
    </section>
  )
}
