import { create } from 'zustand'

interface AuthState {
  isAuthenticated: boolean
  githubUser: string | null
  setAuthenticated: (value: boolean) => void
  setGithubUser: (value: string | null) => void
}

export const useAuthStore = create<AuthState>((set) => ({
  isAuthenticated: false,
  githubUser: null,
  setAuthenticated: (value) => set({ isAuthenticated: value }),
  setGithubUser: (value) => set({ githubUser: value }),
}))
