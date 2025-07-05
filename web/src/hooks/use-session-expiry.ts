'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { useToast } from '@/components/providers/toast-provider'
import { AuthService } from '@/services/auth.service'

interface UseSessionExpiryOptions {
  onSessionExpired?: () => void
  redirectTo?: string
  showModal?: boolean
  showToast?: boolean
}

export const useSessionExpiry = (options: UseSessionExpiryOptions = {}) => {
  const {
    onSessionExpired,
    redirectTo = '/auth/login',
    showModal = true,
    showToast = true
  } = options

  const [isModalOpen, setIsModalOpen] = useState(false)
  const router = useRouter()
  const { showSessionExpiredToast } = useToast()

  const handleSessionExpired = () => {
    // Callback customizado
    onSessionExpired?.()

    if (showToast && !showModal) {
      // Se não vai mostrar modal, mostra toast
      showSessionExpiredToast()
      
      // Redireciona após o toast
      setTimeout(() => {
        router.push(redirectTo)
      }, 4000)
    } else if (showModal) {
      // Mostra modal
      setIsModalOpen(true)
    } else {
      // Redireciona imediatamente
      router.push(redirectTo)
    }
  }

  const handleModalRedirect = () => {
    setIsModalOpen(false)
    // Fazer logout antes de redirecionar
    AuthService.logout()
    router.push(redirectTo)
  }

  // Interceptar respostas 401 da API
  useEffect(() => {
    const handleUnauthorized = () => {
      handleSessionExpired()
    }

    // Escutar evento customizado de 401
    window.addEventListener('session-expired', handleUnauthorized as EventListener)

    return () => {
      window.removeEventListener('session-expired', handleUnauthorized as EventListener)
    }
  }, [])

  return {
    isModalOpen,
    handleModalRedirect,
    triggerSessionExpired: handleSessionExpired
  }
}
