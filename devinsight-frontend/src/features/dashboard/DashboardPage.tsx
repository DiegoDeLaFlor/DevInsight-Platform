import { Link } from 'react-router-dom'

export function DashboardPage() {
  return (
    <section>
      <h1>Repository Dashboard</h1>
      <p>Visualiza repositorios, dispara analisis y revisa estado de calidad.</p>
      <div className="panel">
        <h2>Quick Actions</h2>
        <div className="actions">
          <Link to="/repository">Go to repository details</Link>
          <Link to="/chat">Open copilot-style chat</Link>
        </div>
      </div>
    </section>
  )
}
