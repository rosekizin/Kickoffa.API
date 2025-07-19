'use client'

import { useState, useEffect, useRef, ReactNode } from 'react'
import { Check, ChevronDown } from 'lucide-react'

export interface DropdownOption {
  value: string
  label: string
  description?: string
  icon?: ReactNode
}

interface ElegantDropdownProps {
  options: DropdownOption[]
  value: string
  onChange: (value: string) => void
  placeholder?: string
  disabled?: boolean
  className?: string
}

export const ElegantDropdown = ({
  options,
  value,
  onChange,
  placeholder = "Selecione uma opção...",
  disabled = false,
  className = ""
}: ElegantDropdownProps) => {
  const [isOpen, setIsOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  // Fechar dropdown ao clicar fora
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false)
      }
    }

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside)
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside)
    }
  }, [isOpen])

  const selectedOption = options.find(option => option.value === value)

  const handleSelectOption = (optionValue: string) => {
    onChange(optionValue)
    setIsOpen(false)
  }

  return (
    <div className={`relative ${className}`} ref={dropdownRef}>
      {/* Dropdown trigger */}
      <div className="relative">
        <button
          type="button"
          onClick={() => !disabled && setIsOpen(!isOpen)}
          disabled={disabled}
          className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm text-left bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-2">
              {selectedOption?.icon && (
                <span className="flex-shrink-0">
                  {selectedOption.icon}
                </span>
              )}
              <span className={selectedOption ? 'text-gray-900' : 'text-gray-500'}>
                {selectedOption ? selectedOption.label : placeholder}
              </span>
            </div>
            <ChevronDown className={`h-4 w-4 text-gray-400 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
          </div>
        </button>

        {/* Dropdown content */}
        {isOpen && (
          <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-80 overflow-hidden">
            {/* Options list */}
            <div className="max-h-60 overflow-y-auto">
              {options.map(option => {
                const isSelected = value === option.value

                return (
                  <button
                    key={option.value}
                    type="button"
                    onClick={() => handleSelectOption(option.value)}
                    className={`w-full px-3 py-3 text-left text-sm hover:bg-gray-50 focus:outline-none focus:bg-gray-50 transition-colors ${
                      isSelected ? 'bg-blue-50 text-blue-900' : 'text-gray-900'
                    }`}
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-3">
                        {option.icon && (
                          <div className="flex-shrink-0">
                            {option.icon}
                          </div>
                        )}
                        <div className="flex-1 min-w-0">
                          <div className="font-medium">
                            {option.label}
                          </div>
                          {option.description && (
                            <div className="text-xs text-gray-500 truncate">
                              {option.description}
                            </div>
                          )}
                        </div>
                      </div>
                      {isSelected && (
                        <Check className="h-4 w-4 text-blue-600" />
                      )}
                    </div>
                  </button>
                )
              })}
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
