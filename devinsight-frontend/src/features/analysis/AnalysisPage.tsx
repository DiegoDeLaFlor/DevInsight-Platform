export function AnalysisPage() {
  return (
    <section>
      <h1>Analysis Results</h1>
      <p>Esta vista mostrara issues AST e insights de AI Engine por repositorio.</p>
      <div className="panel">
        <h2>Metrics Preview</h2>
        <ul>
          <li>Long methods detected</li>
          <li>Deep nesting findings</li>
          <li>Large file hotspots</li>
        </ul>
      </div>
    </section>
  )
}
