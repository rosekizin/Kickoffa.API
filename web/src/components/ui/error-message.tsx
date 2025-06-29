import { AlertCircle, XCircle, AlertTriangle, Info } from 'lucide-react'
import { cn } from '@/lib/utils'

interface ErrorMessageProps {
  message: string
  type?: 'error' | 'warning' | 'info'
  title?: string
  className?: string
  onDismiss?: () => void
}

export function ErrorMessage({ 
  message, 
  type = 'error', 
  title, 
  className,
  onDismiss 
}: ErrorMessageProps) {
  const getIcon = () => {
    switch (type) {
      case 'error':
        return <AlertCircle className="h-5 w-5 text-red-600" />
      case 'warning':
        return <AlertTriangle className="h-5 w-5 text-yellow-600" />
      case 'info':
        return <Info className="h-5 w-5 text-blue-600" />
      default:
        return <AlertCircle className="h-5 w-5 text-red-600" />
    }
  }

  const getStyles = () => {
    switch (type) {
      case 'error':
        return {
          container: 'bg-red-50 border-red-200',
          title: 'text-red-800',
          message: 'text-red-700'
        }
      case 'warning':
        return {
          container: 'bg-yellow-50 border-yellow-200',
          title: 'text-yellow-800',
          message: 'text-yellow-700'
        }
      case 'info':
        return {
          container: 'bg-blue-50 border-blue-200',
          title: 'text-blue-800',
          message: 'text-blue-700'
        }
      default:
        return {
          container: 'bg-red-50 border-red-200',
          title: 'text-red-800',
          message: 'text-red-700'
        }
    }
  }

  const styles = getStyles()

  return (
    <div className={cn(
      'border rounded-md p-4',
      styles.container,
      className
    )}>
      <div className="flex items-start">
        <div className="mr-3 mt-0.5 flex-shrink-0">
          {getIcon()}
        </div>
        <div className="flex-1">
          {title && (
            <h3 className={cn('text-sm font-medium mb-1', styles.title)}>
              {title}
            </h3>
          )}
          <p className={cn('text-sm leading-relaxed', styles.message)}>
            {message}
          </p>
        </div>
        {onDismiss && (
          <button
            onClick={onDismiss}
            className="ml-3 flex-shrink-0 text-gray-400 hover:text-gray-600"
          >
            <XCircle className="h-4 w-4" />
          </button>
        )}
      </div>
    </div>
  )
}
