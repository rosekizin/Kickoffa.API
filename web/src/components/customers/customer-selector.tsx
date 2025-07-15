'use client'

import { useState, useEffect, useMemo, useRef } from 'react'
import { Check, ChevronDown, Search, Plus, User, Building } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { useCustomers } from '@/hooks/use-api'
import { CustomerUnion, CustomerType } from '@/types'
import { CustomerService } from '@/services/customer.service'

interface CustomerSelectorProps {
  selectedCustomerId: number | null
  onSelectionChange: (customerId: number | null) => void
  onNewCustomer?: () => void
  placeholder?: string
  disabled?: boolean
}

export const CustomerSelector = ({
  selectedCustomerId,
  onSelectionChange,
  onNewCustomer,
  placeholder = "Buscar cliente...",
  disabled = false
}: CustomerSelectorProps) => {
  const [isOpen, setIsOpen] = useState(false)
  const [searchTerm, setSearchTerm] = useState('')
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

  const { data: customers = [], isLoading } = useCustomers()

  // Filtrar customers baseado no termo de busca
  const filteredCustomers = useMemo(() => {
    return CustomerService.filterBySearchTerm(customers, searchTerm)
  }, [customers, searchTerm])

  const selectedCustomer = customers.find(c => c.id === selectedCustomerId)

  const handleSelectCustomer = (customer: CustomerUnion) => {
    onSelectionChange(customer.id)
    setIsOpen(false)
    setSearchTerm('')
  }

  const handleNewCustomerClick = () => {
    setIsOpen(false)
    setSearchTerm('')
    onNewCustomer?.()
  }

  const getCustomerDisplayName = (customer: CustomerUnion) => {
    const name = CustomerService.getDisplayName(customer)
    const type = customer.type === CustomerType.NaturalPerson ? '(PF)' : '(PJ)'
    return `${name} ${type}`
  }

  return (
    <div className="relative" ref={dropdownRef}>
      {/* Dropdown trigger */}
      <div className="relative">
        <button
          type="button"
          onClick={() => !disabled && setIsOpen(!isOpen)}
          disabled={disabled}
          className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm text-left bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <div className="flex items-center justify-between">
            <span className={selectedCustomer ? 'text-gray-900' : 'text-gray-500'}>
              {selectedCustomer 
                ? getCustomerDisplayName(selectedCustomer)
                : placeholder
              }
            </span>
            <ChevronDown className={`h-4 w-4 text-gray-400 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
          </div>
        </button>

        {/* Dropdown content */}
        {isOpen && (
          <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-80 overflow-hidden">
            {/* Search input */}
            <div className="p-3 border-b border-gray-200">
              <div className="relative">
                <Search className="h-4 w-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar por nome, email, CPF ou CNPJ..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  autoFocus
                />
              </div>
            </div>

            {/* New customer button */}
            {onNewCustomer && (
              <div className="p-2 border-b border-gray-200">
                <button
                  type="button"
                  onClick={handleNewCustomerClick}
                  className="w-full px-3 py-2 text-left text-sm text-blue-600 hover:bg-blue-50 focus:outline-none focus:bg-blue-50 rounded-md flex items-center"
                >
                  <Plus className="h-4 w-4 mr-2" />
                  Criar novo cliente
                </button>
              </div>
            )}

            {/* Customers list */}
            <div className="max-h-60 overflow-y-auto">
              {isLoading ? (
                <div className="p-4 text-center text-gray-500">
                  Carregando clientes...
                </div>
              ) : filteredCustomers.length === 0 ? (
                <div className="p-4 text-center text-gray-500">
                  {searchTerm ? 'Nenhum cliente encontrado' : 'Nenhum cliente cadastrado'}
                </div>
              ) : (
                filteredCustomers.map(customer => {
                  const isSelected = selectedCustomerId === customer.id

                  return (
                    <button
                      key={customer.id}
                      type="button"
                      onClick={() => handleSelectCustomer(customer)}
                      className={`w-full px-3 py-3 text-left text-sm hover:bg-gray-50 focus:outline-none focus:bg-gray-50 ${
                        isSelected ? 'bg-blue-50 text-blue-900' : 'text-gray-900'
                      }`}
                    >
                      <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-3">
                          <div className="flex-shrink-0">
                            {customer.type === CustomerType.LegalCompany ? (
                              <Building className="h-4 w-4 text-gray-400" />
                            ) : (
                              <User className="h-4 w-4 text-gray-400" />
                            )}
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="font-medium">
                              {CustomerService.getDisplayName(customer)}
                            </div>
                            <div className="text-xs text-gray-500 truncate">
                              {customer.email && (
                                <span>{customer.email}</span>
                              )}
                              {CustomerService.getDocument(customer) && (
                                <span className="ml-2">
                                  {customer.type === CustomerType.NaturalPerson ? 'CPF' : 'CNPJ'}: {CustomerService.getDocument(customer)}
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                        {isSelected && (
                          <Check className="h-4 w-4 text-blue-600" />
                        )}
                      </div>
                    </button>
                  )
                })
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
