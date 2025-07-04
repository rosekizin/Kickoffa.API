'use client'

import { Zap } from 'lucide-react'

interface AuthLoadingProps {
  message?: string
  showLogo?: boolean
}

export function AuthLoading({
  message = 'Verificando autenticação...',
  showLogo = true
}: AuthLoadingProps) {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="text-center">
        {showLogo && (
          <div className="mb-8">
            <div className="flex items-center justify-center space-x-3 mb-4">
              <div className="w-12 h-12 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-xl flex items-center justify-center">
                <Zap className="h-7 w-7 text-white" />
              </div>
              <h1 className="text-3xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Kickoffa
              </h1>
            </div>
          </div>
        )}
        
        <div className="bg-white rounded-lg shadow-lg p-8 max-w-sm mx-auto">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-6"></div>
          <p className="text-gray-700 font-medium">{message}</p>
          <p className="text-gray-500 text-sm mt-2">Aguarde um momento...</p>
        </div>
      </div>
    </div>
  )
}
