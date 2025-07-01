'use client'

import { useEffect, useRef, useState } from 'react'

interface UseUserActivityOptions {
  /**
   * Tempo em ms para considerar usuário inativo
   * @default 300000 (5 minutos)
   */
  inactivityThreshold?: number
  
  /**
   * Eventos que indicam atividade do usuário
   * @default ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart', 'click']
   */
  events?: string[]
  
  /**
   * Throttle em ms para evitar muitos eventos
   * @default 1000 (1 segundo)
   */
  throttle?: number
}

export const useUserActivity = (options: UseUserActivityOptions = {}) => {
  const {
    inactivityThreshold = 300000, // 5 minutos
    events = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart', 'click'],
    throttle = 5000 // 5 segundos
  } = options

  const [isActive, setIsActive] = useState(true)
  const [lastActivity, setLastActivity] = useState(Date.now())
  const timeoutRef = useRef<NodeJS.Timeout>()
  const throttleRef = useRef<NodeJS.Timeout>()

  // Função para registrar atividade
  const recordActivity = () => {
    const now = Date.now()
    
    // Throttle para evitar muitas atualizações
    if (throttleRef.current) return
    
    throttleRef.current = setTimeout(() => {
      throttleRef.current = undefined
    }, throttle)

    setLastActivity(now)
    setIsActive(true)

    // Limpar timeout anterior
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
    }

    // Configurar novo timeout para inatividade
    timeoutRef.current = setTimeout(() => {
      setIsActive(false)
    }, inactivityThreshold)
  }

  useEffect(() => {
    // Registrar atividade inicial
    recordActivity()

    // Adicionar listeners para eventos de atividade
    events.forEach(event => {
      document.addEventListener(event, recordActivity, { passive: true })
    })

    // Cleanup
    return () => {
      events.forEach(event => {
        document.removeEventListener(event, recordActivity)
      })
      
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current)
      }
      
      if (throttleRef.current) {
        clearTimeout(throttleRef.current)
      }
    }
  }, [inactivityThreshold, throttle])

  return {
    isActive,
    lastActivity,
    recordActivity
  }
}
