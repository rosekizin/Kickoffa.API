'use client'

import React from 'react'
import { X } from 'lucide-react'

interface DialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  children: React.ReactNode
}

interface DialogContentProps {
  children: React.ReactNode
  className?: string
}

interface DialogHeaderProps {
  children: React.ReactNode
}

interface DialogTitleProps {
  children: React.ReactNode
}

interface DialogDescriptionProps {
  children: React.ReactNode
}

interface DialogFooterProps {
  children: React.ReactNode
}

export const Dialog: React.FC<DialogProps> = ({ open, onOpenChange, children }) => {
  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div 
        className="absolute inset-0 bg-black/50 backdrop-blur-sm" 
        onClick={() => onOpenChange(false)}
      />
      
      {/* Modal Content */}
      {children}
    </div>
  )
}

export const DialogContent: React.FC<DialogContentProps> = ({ children, className = '' }) => {
  // Se className contém max-w-*, não aplicar max-w-md padrão
  const hasMaxWidth = className.includes('max-w-')
  const defaultMaxWidth = hasMaxWidth ? '' : 'max-w-md'

  return (
    <div className={`relative bg-white rounded-lg shadow-xl ${defaultMaxWidth} w-full mx-4 animate-in fade-in-0 zoom-in-95 duration-300 ${className}`}>
      {children}
    </div>
  )
}

export const DialogHeader: React.FC<DialogHeaderProps> = ({ children }) => {
  return (
    <div className="p-6 pb-4">
      {children}
    </div>
  )
}

export const DialogTitle: React.FC<DialogTitleProps> = ({ children }) => {
  return (
    <h3 className="text-lg font-semibold text-gray-900 mb-2">
      {children}
    </h3>
  )
}

export const DialogDescription: React.FC<DialogDescriptionProps> = ({ children }) => {
  return (
    <p className="text-gray-600">
      {children}
    </p>
  )
}

export const DialogFooter: React.FC<DialogFooterProps> = ({ children }) => {
  return (
    <div className="px-6 pb-6 pt-4 flex gap-3 justify-end">
      {children}
    </div>
  )
}
