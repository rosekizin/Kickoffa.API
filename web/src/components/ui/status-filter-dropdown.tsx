'use client'

import { useState } from 'react'
import { ChecklistStatus } from '@/types'
import { ChevronDown, Check, FileText, Edit, Activity, CheckCircle, Archive } from 'lucide-react'

interface StatusFilterDropdownProps {
  selectedStatuses: ChecklistStatus[]
  onChange: (statuses: ChecklistStatus[]) => void
  disabled?: boolean
  className?: string
}

export const StatusFilterDropdown = ({
  selectedStatuses,
  onChange,
  disabled = false,
  className = ""
}: StatusFilterDropdownProps) => {
  const [isOpen, setIsOpen] = useState(false)

  const statusOptions: { value: ChecklistStatus; label: string; color: string; icon: React.ComponentType<any> }[] = [
    { value: 'draft', label: 'Rascunho', color: 'text-gray-600', icon: Edit },
    { value: 'active', label: 'Ativo', color: 'text-blue-500', icon: Activity },
    { value: 'completed', label: 'Concluído', color: 'text-green-600', icon: CheckCircle },
    { value: 'archived', label: 'Arquivado', color: 'text-yellow-600', icon: Archive }
  ]

  const handleStatusToggle = (status: ChecklistStatus) => {
    if (selectedStatuses.includes(status)) {
      onChange(selectedStatuses.filter(s => s !== status))
    } else {
      onChange([...selectedStatuses, status])
    }
  }

  const handleToggleAll = () => {
    if (isAllSelected) {
      // Se todos estão selecionados, desselecionar todos
      onChange([])
    } else {
      // Se nem todos estão selecionados, selecionar todos
      const allStatuses = statusOptions.map(option => option.value)
      onChange(allStatuses)
    }
  }

  const isAllSelected = statusOptions.length === selectedStatuses.length

  const getDisplayText = () => {
    if (selectedStatuses.length === 0) {
      return 'Todos os Status'
    }
    if (selectedStatuses.length === 1) {
      const status = statusOptions.find(s => s.value === selectedStatuses[0])
      return status?.label || 'Status'
    }
    return `${selectedStatuses.length} status selecionados`
  }

  return (
    <div className={`relative ${className}`}>
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        disabled={disabled}
        className={`
          w-full flex items-center justify-between px-3 py-2 text-sm border border-gray-300 rounded-md
          bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500
          ${disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'}
        `}
      >
        <div className="flex items-center">
          <FileText className="h-4 w-4 text-gray-400 mr-2" />
          <span className="text-gray-700">{getDisplayText()}</span>
        </div>
        <ChevronDown className={`h-4 w-4 text-gray-400 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
      </button>

      {isOpen && (
        <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg">
          <div className="py-1">
            {/* Opção "Todos" */}
            <label className="flex items-center px-3 py-2 hover:bg-gray-50 cursor-pointer border-b border-gray-100">
              <input
                type="checkbox"
                checked={isAllSelected}
                onChange={handleToggleAll}
                className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
              />
              <div className="ml-3 flex items-center">
                <FileText className="h-4 w-4 text-gray-500 mr-2" />
                <span className="text-sm font-medium text-gray-700">
                  Todos os Status
                </span>
                {isAllSelected && (
                  <Check className="h-4 w-4 text-blue-600 ml-2" />
                )}
              </div>
            </label>

            {/* Opções individuais */}
            {statusOptions.map((option) => {
              const IconComponent = option.icon
              return (
                <label
                  key={option.value}
                  className="flex items-center px-3 py-2 hover:bg-gray-50 cursor-pointer"
                >
                  <input
                    type="checkbox"
                    checked={selectedStatuses.includes(option.value)}
                    onChange={() => handleStatusToggle(option.value)}
                    className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                  <div className="ml-3 flex items-center">
                    <IconComponent className={`h-4 w-4 mr-2 ${option.color}`} />
                    <span className={`text-sm font-medium ${option.color}`}>
                      {option.label}
                    </span>
                    {selectedStatuses.includes(option.value) && (
                      <Check className="h-4 w-4 text-blue-600 ml-2" />
                    )}
                  </div>
                </label>
              )
            })}
          </div>
          
          {selectedStatuses.length > 0 && (
            <div className="border-t border-gray-200 px-3 py-2">
              <button
                type="button"
                onClick={() => onChange([])}
                className="text-xs text-gray-500 hover:text-gray-700"
              >
                Limpar seleção
              </button>
            </div>
          )}
        </div>
      )}

      {/* Overlay para fechar o dropdown */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40"
          onClick={() => setIsOpen(false)}
        />
      )}
    </div>
  )
}
