'use client'

import { useState } from 'react'
import { ChevronDown, ChevronRight } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface CollapsibleSectionProps {
  title: string
  children: React.ReactNode
  defaultExpanded?: boolean
  className?: string
  headerClassName?: string
  contentClassName?: string
  icon?: React.ReactNode
}

export function CollapsibleSection({
  title,
  children,
  defaultExpanded = true,
  className = '',
  headerClassName = '',
  contentClassName = '',
  icon
}: CollapsibleSectionProps) {
  const [isExpanded, setIsExpanded] = useState(defaultExpanded)

  return (
    <div className={`border-b border-gray-200 ${className}`}>
      <Button
        variant="ghost"
        onClick={() => setIsExpanded(!isExpanded)}
        className={`w-full justify-between p-4 h-auto hover:bg-gray-50 ${headerClassName}`}
      >
        <div className="flex items-center space-x-2">
          {icon && <span className="text-gray-500">{icon}</span>}
          <span className="font-medium text-gray-900 text-left">{title}</span>
        </div>
        
        <div className="flex items-center space-x-2">
          {isExpanded ? (
            <ChevronDown className="h-4 w-4 text-gray-500" />
          ) : (
            <ChevronRight className="h-4 w-4 text-gray-500" />
          )}
        </div>
      </Button>
      
      {isExpanded && (
        <div className={`pb-4 px-4 ${contentClassName}`}>
          {children}
        </div>
      )}
    </div>
  )
}
