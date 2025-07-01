'use client'

import { useEffect, useRef } from 'react'
import { useUserActivity } from './use-user-activity'
import { AuthService } from '@/services/auth.service'

interface UseSessionKeepAliveOptions {
  /**
   * Intervalo em ms para enviar ping quando usuário está ativo
   * @default 900000 (15 minutos - metade dos 30min de expiração)
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
   * Se deve fazer log das atividades (para debug)
   * @default false
   */
  enableLogging?: boolean
}

export const useSessionKeepAlive = (options: UseSessionKeepAliveOptions = {}) => {
  const {
    pingInterval = 900000, // 15 minutos (metade dos 30min de expiração)
    inactivityThreshold = 300000, // 5 minutos
    onlyWhenActive = true,
    enableLogging = false
  } = options

  const { isActive, lastActivity } = useUserActivity({
    inactivityThreshold,
    throttle: 5000 // 5 segundos de throttle para atividade
  })
  
  const pingIntervalRef = useRef<NodeJS.Timeout>()
  const lastPingRef = useRef<number>(0)

  // Função para enviar ping ao servidor
  const sendPing = async (): Promise<boolean> => {
    try {
      // Verificar se usuário está autenticado antes de fazer ping
      if (!AuthService.isAuthenticated()) {
        if (enableLogging) {
          console.log('🔄 KeepAlive: Usuário não autenticado, pulando ping')
        }
        return false
      }

      if (enableLogging) {
        console.log('🔄 KeepAlive: Enviando ping para manter sessão viva...')
      }

      const isValid = await AuthService.validateToken()
      lastPingRef.current = Date.now()

      if (enableLogging) {
        console.log(`✅ KeepAlive: Ping ${isValid ? 'bem-sucedido' : 'falhou'}`)
      }

      return isValid
    } catch (error) {
      if (enableLogging) {
        console.error('❌ KeepAlive: Erro no ping:', error)
      }
      return false
    }
  }

  // Função para verificar se deve fazer ping
  const shouldPing = (): boolean => {
    // Se só deve fazer ping quando ativo e usuário está inativo
    if (onlyWhenActive && !isActive) {
      if (enableLogging) {
        console.log('🔄 KeepAlive: Usuário inativo, pulando ping')
      }
      return false
    }

    // Verificar se já passou tempo suficiente desde último ping
    const timeSinceLastPing = Date.now() - lastPingRef.current
    if (timeSinceLastPing < pingInterval) {
      return false
    }

    return true
  }

  // Configurar intervalo de ping
  useEffect(() => {
    const startPingInterval = () => {
      // Limpar intervalo anterior se existir
      if (pingIntervalRef.current) {
        clearInterval(pingIntervalRef.current)
      }

      // Configurar novo intervalo
      pingIntervalRef.current = setInterval(async () => {
        if (shouldPing()) {
          await sendPing()
        }
      }, pingInterval)

      if (enableLogging) {
        console.log(`🔄 KeepAlive: Iniciado com intervalo de ${pingInterval / 60000} minutos`)
      }
    }

    // Iniciar apenas se usuário está autenticado
    if (AuthService.isAuthenticated()) {
      startPingInterval()
    }

    // Cleanup
    return () => {
      if (pingIntervalRef.current) {
        clearInterval(pingIntervalRef.current)
        if (enableLogging) {
          console.log('🔄 KeepAlive: Intervalo limpo')
        }
      }
    }
  }, [pingInterval, onlyWhenActive, enableLogging])

  // Fazer ping imediato quando usuário volta a ficar ativo
  useEffect(() => {
    if (isActive && onlyWhenActive) {
      const timeSinceLastActivity = Date.now() - lastActivity
      const timeSinceLastPing = Date.now() - lastPingRef.current
      
      // Se usuário ficou inativo por mais tempo que o intervalo de ping
      // e já passou tempo suficiente desde último ping, fazer ping imediato
      if (timeSinceLastActivity > inactivityThreshold && timeSinceLastPing > pingInterval) {
        if (enableLogging) {
          console.log('🔄 KeepAlive: Usuário voltou a ficar ativo, fazendo ping imediato')
        }
        sendPing()
      }
    }
  }, [isActive, lastActivity])

  return {
    isActive,
    lastActivity,
    lastPing: lastPingRef.current,
    sendPing
  }
}
