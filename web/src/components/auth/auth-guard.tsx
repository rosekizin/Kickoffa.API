'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { AuthService } from '@/services/auth.service'

interface AuthGuardProps {
  children: React.ReactNode
  requireAuth?: boolean
  redirectTo?: string
}

export function AuthGuard({
  children,
  requireAuth = true,
  redirectTo = '/auth/login'
}: AuthGuardProps) {
  const [isLoading, setIsLoading] = useState(true)
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const router = useRouter()

  // Função para disparar notificação de sessão expirada
  const triggerSessionExpiredNotification = (source: string) => {
    if (typeof window !== 'undefined') {
      console.warn(`🔒 AuthGuard (${source}): Disparando evento de sessão expirada`)

      const sessionExpiredEvent = new CustomEvent('session-expired', {
        detail: {
          status: 401,
          message: 'Sessão expirada',
          timestamp: new Date().toISOString(),
          source: `AuthGuard-${source}`
        }
      })
      window.dispatchEvent(sessionExpiredEvent)
    }
  }

  useEffect(() => {
    const checkAuth = async () => {
      try {
        console.log('🔍 AuthGuard: Verificando autenticação...')
        const authenticated = AuthService.isAuthenticated()
        console.log('🔍 AuthGuard: isAuthenticated =', authenticated)

        if (authenticated) {
          console.log('🔍 AuthGuard: Usuário tem dados locais, assumindo autenticado')
          // Se tem dados do usuário, assumir autenticado inicialmente
          setIsAuthenticated(true)

          // Validar token com o servidor em background (não bloquear acesso)
          console.log('🔍 AuthGuard: Validando token com servidor em background...')
          AuthService.validateToken().then(isValid => {
            console.log('🔍 AuthGuard: Validação em background - Token válido =', isValid)
            if (!isValid) {
              console.log('🔍 AuthGuard: Token inválido, fazendo logout')
              AuthService.logout()
              if (requireAuth) {
                triggerSessionExpiredNotification('token-validation')
                // Não redirecionar imediatamente - deixar o SessionManager cuidar disso
              }
            }
          }).catch(error => {
            console.error('❌ AuthGuard: Erro na validação em background:', error)
          })
        } else {
          console.log('🔍 AuthGuard: Usuário não autenticado')
          setIsAuthenticated(false)

          if (requireAuth) {
            console.log('🔍 AuthGuard: Usuário não autenticado, disparando notificação')
            triggerSessionExpiredNotification('not-authenticated')
            return
          }
        }
      } catch (error) {
        console.error('❌ AuthGuard: Erro ao verificar autenticação:', error)
        setIsAuthenticated(false)

        if (requireAuth) {
          console.log('🔍 AuthGuard: Erro na autenticação, disparando notificação')
          triggerSessionExpiredNotification('auth-error')
          return
        }
      } finally {
        console.log('🔍 AuthGuard: Finalizando verificação, loading = false')
        setIsLoading(false)
      }
    }

    checkAuth()
  }, [requireAuth, redirectTo, router])

  // Mostrar loading enquanto verifica autenticação
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Verificando autenticação...</p>
        </div>
      </div>
    )
  }

  // Se requer autenticação mas não está autenticado, não renderizar nada
  // (o redirecionamento já foi feito)
  if (requireAuth && !isAuthenticated) {
    return null
  }

  // Se não requer autenticação ou está autenticado, renderizar children
  return <>{children}</>
}
