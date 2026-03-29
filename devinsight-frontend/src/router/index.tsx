import { createBrowserRouter } from 'react-router-dom'
import { AnalysisPage } from '../features/analysis/AnalysisPage'
import { ChatPage } from '../features/chat/ChatPage'
import { DashboardPage } from '../features/dashboard/DashboardPage'
import { RepositoryPage } from '../features/repository/RepositoryPage'
import { AppLayout } from '../layouts/AppLayout'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'repository', element: <RepositoryPage /> },
      { path: 'analysis', element: <AnalysisPage /> },
      { path: 'chat', element: <ChatPage /> },
    ],
  },
])
