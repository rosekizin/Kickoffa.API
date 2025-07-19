'use client'

import { ElegantDropdown, DropdownOption } from './elegant-dropdown'
import { FileText, CheckCircle, Edit, Archive } from 'lucide-react'

interface StatusDropdownProps {
  value: string
  onChange: (value: string) => void
  disabled?: boolean
  className?: string
}

export const StatusDropdown = ({
  value,
  onChange,
  disabled = false,
  className = ""
}: StatusDropdownProps) => {
  const statusOptions: DropdownOption[] = [
    {
      value: 'all',
      label: 'Todos os Status',
      description: 'Mostrar checklists com qualquer status',
      icon: <FileText className="h-4 w-4 text-gray-400" />
    },
    {
      value: 'draft',
      label: 'Rascunho',
      description: 'Checklists ainda em desenvolvimento',
      icon: <Edit className="h-4 w-4 text-gray-400" />
    },
    {
      value: 'active',
      label: 'Ativo',
      description: 'Checklists publicados e em uso',
      icon: <CheckCircle className="h-4 w-4 text-blue-500" />
    },
    {
      value: 'completed',
      label: 'Concluído',
      description: 'Checklists finalizados',
      icon: <CheckCircle className="h-4 w-4 text-green-500" />
    },
    {
      value: 'archived',
      label: 'Arquivado',
      description: 'Checklists arquivados',
      icon: <Archive className="h-4 w-4 text-yellow-500" />
    }
  ]

  return (
    <ElegantDropdown
      options={statusOptions}
      value={value}
      onChange={onChange}
      placeholder="Filtrar por status..."
      disabled={disabled}
      className={className}
    />
  )
}
