import { Link, Outlet } from 'react-router-dom'

export function AppLayout() {
  return (
    <div className="app-shell">
      <header className="topbar">
        <h1>DevInsight</h1>
        <nav>
          <Link to="/">Dashboard</Link>
          <Link to="/repository">Repository</Link>
          <Link to="/analysis">Analysis</Link>
          <Link to="/chat">Chat</Link>
        </nav>
      </header>
      <main className="content">
        <Outlet />
      </main>
    </div>
  )
}
