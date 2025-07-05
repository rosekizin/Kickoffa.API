'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { useAuth } from '@/contexts/auth-context'
import { AuthService } from '@/services/auth.service'
import { AuthLoading } from './auth-loading'

interface AuthGuardProps {
  children: React.ReactNode
  requireAuth?: boolean
  redirectTo?: string
  showLoadingOnNavigation?: boolean
}

export function AuthGuard({
  children,
  requireAuth = true,
  redirectTo = '/auth/login',
  showLoadingOnNavigation = false
}: AuthGuardProps) {
  const { isAuthenticated, isInitializing } = useAuth()
  const [isNavigationLoading, setIsNavigationLoading] = useState(false)
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
        // Se ainda está inicializando, aguardar
        if (isInitializing) {
          return
        }

        // Se não requer autenticação, não fazer nada
        if (!requireAuth) {
          return
        }

        // Verificar se há dados de usuário localmente
        if (AuthService.isAuthenticated()) {
          // Validar token no servidor
          try {
            const isValid = await AuthService.validateToken()

            if (!isValid) {
              console.log('🔍 AuthGuard: Token inválido - sessão expirada')
              // NÃO fazer logout imediatamente - deixar o modal controlar
              triggerSessionExpiredNotification('token-validation')
              return
            }
          } catch (validationError) {
            console.error('❌ AuthGuard: Erro na validação do token:', validationError)
            // NÃO fazer logout imediatamente - deixar o modal controlar
            triggerSessionExpiredNotification('validation-error')
            return
          }
        } else {
          // Se não está autenticado e requer autenticação, redirecionar
          if (showLoadingOnNavigation) {
            setIsNavigationLoading(true)
          }

          try {
            router.push(redirectTo)
          } catch (error) {
            console.error('Erro no redirecionamento, usando window.location.href', error)
            window.location.href = redirectTo
          }
        }
      } catch (error) {
        console.error('❌ AuthGuard: Erro geral ao verificar autenticação:', error)

        if (requireAuth) {
          try {
            router.push(redirectTo)
          } catch (routerError) {
            console.error('❌ AuthGuard: Erro no router.push, usando window.location.href', routerError)
            window.location.href = redirectTo
          }
        }
      }
    }

    checkAuth()
  }, [isAuthenticated, isInitializing, requireAuth, redirectTo, router, showLoadingOnNavigation])

  // Mostrar loading apenas durante inicialização
  if (isInitializing) {
    return <AuthLoading message="Verificando autenticação..." />
  }

  // Loading simples para navegação (se solicitado)
  if (isNavigationLoading && showLoadingOnNavigation) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
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
