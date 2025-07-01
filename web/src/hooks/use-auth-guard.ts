'use client'

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useToast } from '@/components/providers/toast-provider'

interface UseAuthGuardOptions {
  redirectTo?: string
  showNotification?: boolean
  checkInterval?: number
}

export const useAuthGuard = (options: UseAuthGuardOptions = {}) => {
  const {
    redirectTo = '/login',
    showNotification = true,
    checkInterval = 60000 // 1 minuto
  } = options

  const router = useRouter()
  const { showToast } = useToast()

  useEffect(() => {
    // Verificar periodicamente se o usuário ainda está autenticado
    const checkAuth = async () => {
      try {
        // Fazer uma requisição simples para verificar se ainda está autenticado
        const response = await fetch('/api/auth/validate', {
          credentials: 'include'
        })

        if (!response.ok && response.status === 401) {
          if (showNotification) {
            showToast({
              title: 'Sessão Expirada',
              description: 'Sua sessão expirou. Faça login novamente.',
              type: 'warning',
              duration: 3000
            })
          }
          
          setTimeout(() => {
            router.push(redirectTo)
          }, 3000)
        }
      } catch (error) {
        console.error('Erro ao verificar autenticação:', error)
      }
    }

    // Verificar imediatamente
    checkAuth()

    // Configurar verificação periódica
    const interval = setInterval(checkAuth, checkInterval)

    return () => clearInterval(interval)
  }, [router, redirectTo, showNotification, checkInterval, showToast])

  // Verificar quando a aba volta ao foco
  useEffect(() => {
    const handleVisibilityChange = () => {
      if (!document.hidden) {
        // Verificar autenticação quando a aba volta ao foco
        setTimeout(async () => {
          try {
            const response = await fetch('/api/auth/validate', {
              credentials: 'include'
            })

            if (!response.ok && response.status === 401) {
              if (showNotification) {
                showToast({
                  title: 'Sessão Expirada',
                  description: 'Sua sessão expirou enquanto você estava ausente.',
                  type: 'warning',
                  duration: 4000
                })
              }
              
              setTimeout(() => {
                router.push(redirectTo)
              }, 4000)
            }
          } catch (error) {
            console.error('Erro ao verificar autenticação no foco:', error)
          }
        }, 1000)
      }
    }

    document.addEventListener('visibilitychange', handleVisibilityChange)

    return () => {
      document.removeEventListener('visibilitychange', handleVisibilityChange)
    }
  }, [router, redirectTo, showNotification, showToast])
}
