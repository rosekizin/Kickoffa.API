'use client'

import React from 'react'
import { useSessionKeepAlive } from '@/hooks/use-session-keep-alive'

interface SessionKeepAliveProps {
  /**
   * Intervalo em ms para enviar ping quando usuário está ativo
   * @default 900000 (15 minutos)
   */
  pingInterval?: number

  /**
   * Tempo em ms para considerar usuário inativo
   * @default 300000 (5 minutos)
   */
  inactivityThreshold?: number
  
  /**
   * Se deve fazer ping apenas quando usuário está ativo
   * @default true
   */
  onlyWhenActive?: boolean
  
  /**
   * Se deve mostrar logs para debug
   * @default false
   */
  enableLogging?: boolean
  
  /**
   * Se deve mostrar indicador visual de atividade (para debug)
   * @default false
   */
  showActivityIndicator?: boolean
}

export const SessionKeepAlive: React.FC<SessionKeepAliveProps> = ({
  pingInterval = 900000, // 15 minutos (metade dos 30min de expiração)
  inactivityThreshold = 300000, // 5 minutos
  onlyWhenActive = true,
  enableLogging = false,
  showActivityIndicator = false
}) => {
  const { isActive, lastActivity, lastPing } = useSessionKeepAlive({
    pingInterval,
    inactivityThreshold,
    onlyWhenActive,
    enableLogging
  })

  // Componente invisível por padrão
  if (!showActivityIndicator) {
    return null
  }

  // Indicador visual para debug (apenas em desenvolvimento)
  return (
    <div className="fixed top-2 right-2 z-50 bg-black/80 text-white text-xs p-2 rounded font-mono">
      <div className="flex items-center gap-2">
        <div className={`w-2 h-2 rounded-full ${isActive ? 'bg-green-400' : 'bg-red-400'}`} />
        <span>{isActive ? 'Ativo' : 'Inativo'}</span>
      </div>
      <div className="text-gray-300 mt-1">
        <div>Última atividade: {new Date(lastActivity).toLocaleTimeString()}</div>
        {lastPing > 0 && (
          <div>Último ping: {new Date(lastPing).toLocaleTimeString()}</div>
        )}
      </div>
    </div>
  )
}
