'use client'

import React from 'react'
import { useSessionExpiry } from '@/hooks/use-session-expiry'
import { SessionExpiredModal } from '@/components/ui/session-expired-modal'
import { SessionKeepAlive } from './session-keep-alive'

interface SessionManagerProps {
  children: React.ReactNode
  showModal?: boolean
  showToast?: boolean
  /**
   * Se deve ativar keep-alive da sessão
   * @default true
   */
  enableKeepAlive?: boolean
  /**
   * Intervalo em ms para ping de keep-alive
   * @default 900000 (15 minutos)
   */
  keepAlivePingInterval?: number
  /**
   * Se deve mostrar logs de debug do keep-alive
   * @default false
   */
  enableKeepAliveLogging?: boolean
}

export const SessionManager: React.FC<SessionManagerProps> = ({
  children,
  showModal = true,
  showToast = true,
  enableKeepAlive = true,
  keepAlivePingInterval = 900000, // 15 minutos
  enableKeepAliveLogging = false
}) => {
  const { isModalOpen, handleModalRedirect } = useSessionExpiry({
    showModal,
    showToast,
    redirectTo: '/auth/login',
    onSessionExpired: () => {
      console.log('🔒 Sessão expirada - usuário será redirecionado')
    }
  })

  return (
    <>
      {children}

      {/* Keep-alive da sessão */}
      {enableKeepAlive && (
        <SessionKeepAlive
          pingInterval={keepAlivePingInterval}
          enableLogging={enableKeepAliveLogging}
          onlyWhenActive={true}
          inactivityThreshold={300000} // 5 minutos
        />
      )}

      {/* Modal de sessão expirada */}
      <SessionExpiredModal
        isOpen={isModalOpen}
        onRedirect={handleModalRedirect}
        countdown={5}
      />
    </>
  )
}
