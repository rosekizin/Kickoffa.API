'use client'

import React, { useEffect, useState } from 'react'
import { AlertTriangle, LogOut } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface SessionExpiredModalProps {
  isOpen: boolean
  onRedirect: () => void
  countdown?: number
}

export const SessionExpiredModal: React.FC<SessionExpiredModalProps> = ({
  isOpen,
  onRedirect,
  countdown = 5
}) => {
  const [timeLeft, setTimeLeft] = useState(countdown)

  useEffect(() => {
    if (!isOpen) {
      setTimeLeft(countdown)
      return
    }

    const timer = setInterval(() => {
      setTimeLeft(prev => {
        if (prev <= 1) {
          onRedirect()
          return 0
        }
        return prev - 1
      })
    }, 1000)

    return () => clearInterval(timer)
  }, [isOpen, countdown, onRedirect])

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" />
      
      {/* Modal */}
      <div className="relative bg-white rounded-lg shadow-xl max-w-md w-full mx-4 animate-in fade-in-0 zoom-in-95 duration-300">
        <div className="p-6">
          {/* Icon */}
          <div className="flex items-center justify-center w-12 h-12 mx-auto mb-4 bg-yellow-100 rounded-full">
            <AlertTriangle className="w-6 h-6 text-yellow-600" />
          </div>
          
          {/* Content */}
          <div className="text-center">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">
              Sessão Expirada
            </h3>
            <p className="text-gray-600 mb-6">
              Sua sessão expirou por motivos de segurança. Você será redirecionado para a página de login.
            </p>
            
            {/* Countdown */}
            <div className="mb-6">
              <div className="inline-flex items-center px-3 py-2 bg-yellow-50 border border-yellow-200 rounded-md">
                <LogOut className="w-4 h-4 text-yellow-600 mr-2" />
                <span className="text-sm text-yellow-800">
                  Redirecionando em <strong>{timeLeft}</strong> segundo{timeLeft !== 1 ? 's' : ''}
                </span>
              </div>
            </div>
            
            {/* Actions */}
            <div className="flex gap-3 justify-center">
              <Button
                onClick={onRedirect}
                className="flex items-center gap-2"
              >
                <LogOut className="w-4 h-4" />
                Ir para Login Agora
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
