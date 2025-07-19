'use client'

import { ElegantDropdown, DropdownOption } from './elegant-dropdown'
import { List } from 'lucide-react'

interface PageSizeDropdownProps {
  value: number
  onChange: (value: number) => void
  disabled?: boolean
  className?: string
}

export const PageSizeDropdown = ({
  value,
  onChange,
  disabled = false,
  className = ""
}: PageSizeDropdownProps) => {
  const pageSizeOptions: DropdownOption[] = [
    {
      value: '10',
      label: '10 por página',
      description: 'Visualização compacta',
      icon: <List className="h-4 w-4 text-gray-400" />
    },
    {
      value: '20',
      label: '20 por página',
      description: 'Visualização padrão',
      icon: <List className="h-4 w-4 text-gray-400" />
    },
    {
      value: '30',
      label: '30 por página',
      description: 'Visualização expandida',
      icon: <List className="h-4 w-4 text-gray-400" />
    },
    {
      value: '50',
      label: '50 por página',
      description: 'Visualização extensa',
      icon: <List className="h-4 w-4 text-gray-400" />
    },
    {
      value: '100',
      label: '100 por página',
      description: 'Visualização máxima',
      icon: <List className="h-4 w-4 text-gray-400" />
    }
  ]

  const handleChange = (newValue: string) => {
    onChange(Number(newValue))
  }

  return (
    <ElegantDropdown
      options={pageSizeOptions}
      value={value.toString()}
      onChange={handleChange}
      placeholder="Itens por página..."
      disabled={disabled}
      className={className}
    />
  )
}
