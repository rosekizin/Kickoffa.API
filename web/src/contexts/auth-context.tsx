'use client'

import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import { AuthService } from '@/services/auth.service'

interface AuthContextType {
  isAuthenticated: boolean
  isInitializing: boolean
  user: any | null
  checkAuth: () => Promise<void>
  setAuthenticated: (authenticated: boolean, user?: any) => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [isInitializing, setIsInitializing] = useState(true)
  const [user, setUser] = useState<any | null>(null)
  
  const checkAuth = async () => {
    try {
      // Delay mínimo para garantir que o loading seja visível
      await new Promise(resolve => setTimeout(resolve, 800))

      // Verificar se há dados locais primeiro (sem validação de token)
      const localAuth = AuthService.isAuthenticated()
      const localUser = AuthService.getUser()

      if (localAuth && localUser) {
        setIsAuthenticated(true)
        setUser(localUser)
      } else {
        setIsAuthenticated(false)
        setUser(null)
      }
    } catch (error) {
      console.error('Erro ao verificar autenticação:', error)
      setIsAuthenticated(false)
      setUser(null)
    } finally {
      setIsInitializing(false)
    }
  }

  const setAuthenticated = (authenticated: boolean, userData?: unknown) => {
    setIsAuthenticated(authenticated)
    setUser(userData || null)
  }

  useEffect(() => {
    checkAuth()

    // Escutar eventos de login e logout
    const handleLogin = () => {
      // Mostrar loading temporariamente após login
      setIsInitializing(true)

      // Definir usuário como autenticado após um tempo menor
      setTimeout(() => {
        const userData = AuthService.getUser()
        setIsAuthenticated(true)
        setUser(userData)
      }, 400) // 400ms para definir como autenticado

      // Manter loading por mais tempo para transição suave
      setTimeout(() => {
        setIsInitializing(false)
      }, 1000) // 1000ms total de loading
    }

    const handleLogout = () => {
      setIsAuthenticated(false)
      setUser(null)
    }

    window.addEventListener('auth-login', handleLogin as EventListener)
    window.addEventListener('auth-logout', handleLogout)

    return () => {
      window.removeEventListener('auth-login', handleLogin as EventListener)
      window.removeEventListener('auth-logout', handleLogout)
    }
  }, [])

  return (
    <AuthContext.Provider value={{
      isAuthenticated,
      isInitializing,
      user,
      checkAuth,
      setAuthenticated
    }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
