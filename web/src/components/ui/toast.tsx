'use client'

import * as React from 'react'
import { X, AlertCircle, CheckCircle, Info, AlertTriangle } from 'lucide-react'
import { cn } from '@/lib/utils'

export interface ToastProps {
  id: string
  title?: string
  description?: string
  type?: 'success' | 'error' | 'warning' | 'info'
  duration?: number
  onClose?: () => void
}

const Toast = React.forwardRef<HTMLDivElement, ToastProps>(
  ({ id, title, description, type = 'info', duration = 5000, onClose }, ref) => {
    const [isVisible, setIsVisible] = React.useState(true)
    const [isExiting, setIsExiting] = React.useState(false)

    React.useEffect(() => {
      if (duration > 0) {
        const timer = setTimeout(() => {
          handleClose()
        }, duration)

        return () => clearTimeout(timer)
      }
    }, [duration])

    const handleClose = () => {
      setIsExiting(true)
      setTimeout(() => {
        setIsVisible(false)
        onClose?.()
      }, 300) // Animation duration
    }

    if (!isVisible) return null

    const getIcon = () => {
      switch (type) {
        case 'success':
          return <CheckCircle className="h-5 w-5 text-green-600" />
        case 'error':
          return <AlertCircle className="h-5 w-5 text-red-600" />
        case 'warning':
          return <AlertTriangle className="h-5 w-5 text-yellow-600" />
        default:
          return <Info className="h-5 w-5 text-blue-600" />
      }
    }

    const getStyles = () => {
      switch (type) {
        case 'success':
          return 'border-green-200 bg-green-50'
        case 'error':
          return 'border-red-200 bg-red-50'
        case 'warning':
          return 'border-yellow-200 bg-yellow-50'
        default:
          return 'border-blue-200 bg-blue-50'
      }
    }

    return (
      <div
        ref={ref}
        className={cn(
          'pointer-events-auto w-full max-w-sm overflow-hidden rounded-lg border shadow-lg transition-all duration-300',
          getStyles(),
          isExiting 
            ? 'translate-x-full opacity-0' 
            : 'translate-x-0 opacity-100'
        )}
      >
        <div className="p-4">
          <div className="flex items-start">
            <div className="flex-shrink-0">
              {getIcon()}
            </div>
            <div className="ml-3 w-0 flex-1">
              {title && (
                <p className="text-sm font-medium text-gray-900">
                  {title}
                </p>
              )}
              {description && (
                <p className={cn(
                  "text-sm text-gray-500",
                  title ? "mt-1" : ""
                )}>
                  {description}
                </p>
              )}
            </div>
            <div className="ml-4 flex flex-shrink-0">
              <button
                type="button"
                className="inline-flex rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                onClick={handleClose}
              >
                <span className="sr-only">Fechar</span>
                <X className="h-5 w-5" />
              </button>
            </div>
          </div>
        </div>
      </div>
    )
  }
)
Toast.displayName = 'Toast'

export { Toast }
