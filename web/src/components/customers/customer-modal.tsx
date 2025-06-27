'use client'

import { useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import { CustomerService } from '@/services/customer.service'
import { useCreateCustomer, useUpdateCustomer } from '@/hooks/use-api'
import { Customer, CreateCustomerRequest } from '@/types'
import { X, User, Building, AlertCircle } from 'lucide-react'

interface CustomerModalProps {
  isOpen: boolean
  onClose: () => void
  onSave: (customer: Customer) => void
  customer?: Customer
}

export function CustomerModal({ isOpen, onClose, onSave, customer }: CustomerModalProps) {
  const [formData, setFormData] = useState<CreateCustomerRequest>({
    firstName: '',
    lastName: '',
    email: '',
    cpf: '',
    cnpj: '',
    phoneNumber: '',
    address: ''
  })
  const [customerType, setCustomerType] = useState<'pf' | 'pj'>('pf')
  const [errors, setErrors] = useState<Record<string, string>>({})

  // Hooks do TanStack Query
  const createCustomerMutation = useCreateCustomer()
  const updateCustomerMutation = useUpdateCustomer()

  useEffect(() => {
    if (customer) {
      setFormData({
        firstName: customer.firstName,
        lastName: customer.lastName,
        email: customer.email || '',
        cpf: customer.cpf || '',
        cnpj: customer.cnpj || '',
        phoneNumber: customer.phoneNumber || '',
        address: customer.address || ''
      })
      setCustomerType(customer.cnpj ? 'pj' : 'pf')
    } else {
      setFormData({
        firstName: '',
        lastName: '',
        email: '',
        cpf: '',
        cnpj: '',
        phoneNumber: '',
        address: ''
      })
      setCustomerType('pf')
    }
    setErrors({})
  }, [customer, isOpen])

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}

    if (!formData.firstName.trim()) {
      newErrors.firstName = 'Nome é obrigatório'
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = 'Sobrenome é obrigatório'
    }

    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email inválido'
    }

    if (customerType === 'pf' && formData.cpf) {
      if (!CustomerService.validateCpf(formData.cpf)) {
        newErrors.cpf = 'CPF inválido'
      }
    }

    if (customerType === 'pj' && formData.cnpj) {
      if (!CustomerService.validateCnpj(formData.cnpj)) {
        newErrors.cnpj = 'CNPJ inválido'
      }
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) return

    try {
      // Preparar dados baseado no tipo de cliente
      const dataToSend: CreateCustomerRequest = {
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email.trim() || undefined,
        phoneNumber: formData.phoneNumber.trim() || undefined,
        address: formData.address.trim() || undefined,
        cpf: customerType === 'pf' ? formData.cpf.replace(/\D/g, '') || undefined : undefined,
        cnpj: customerType === 'pj' ? formData.cnpj.replace(/\D/g, '') || undefined : undefined
      }

      let savedCustomer: Customer
      if (customer) {
        savedCustomer = await updateCustomerMutation.mutateAsync({ id: customer.id, data: dataToSend })
      } else {
        savedCustomer = await createCustomerMutation.mutateAsync(dataToSend)
      }

      onSave(savedCustomer)
      onClose()
    } catch (error) {
      console.error('Erro ao salvar cliente:', error)
      setErrors({ general: error instanceof Error ? error.message : 'Erro ao salvar cliente' })
    }
  }

  const isLoading = createCustomerMutation.isPending || updateCustomerMutation.isPending

  const handleInputChange = (field: keyof CreateCustomerRequest, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }))
    
    // Limpar erro do campo quando usuário começar a digitar
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }))
    }
  }

  const formatCpfInput = (value: string) => {
    const numbers = value.replace(/\D/g, '')
    return numbers.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
  }

  const formatCnpjInput = (value: string) => {
    const numbers = value.replace(/\D/g, '')
    return numbers.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5')
  }

  const formatPhoneInput = (value: string) => {
    const numbers = value.replace(/\D/g, '')
    if (numbers.length <= 10) {
      return numbers.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3')
    }
    return numbers.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">
            {customer ? 'Editar Cliente' : 'Novo Cliente'}
          </h2>
          <Button variant="ghost" size="sm" onClick={onClose}>
            <X className="h-4 w-4" />
          </Button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-6">
          {/* Erro geral */}
          {errors.general && (
            <div className="bg-red-50 border border-red-200 rounded-md p-3 flex items-center">
              <AlertCircle className="h-4 w-4 text-red-600 mr-2" />
              <span className="text-sm text-red-600">{errors.general}</span>
            </div>
          )}

          {/* Tipo de Cliente */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-3">
              Tipo de Cliente
            </label>
            <div className="grid grid-cols-2 gap-3">
              <button
                type="button"
                onClick={() => setCustomerType('pf')}
                className={`p-4 border rounded-lg flex items-center justify-center space-x-2 transition-colors ${
                  customerType === 'pf'
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
              >
                <User className="h-5 w-5" />
                <span>Pessoa Física</span>
              </button>
              <button
                type="button"
                onClick={() => setCustomerType('pj')}
                className={`p-4 border rounded-lg flex items-center justify-center space-x-2 transition-colors ${
                  customerType === 'pj'
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
              >
                <Building className="h-5 w-5" />
                <span>Pessoa Jurídica</span>
              </button>
            </div>
          </div>

          {/* Nome e Sobrenome */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Nome *
              </label>
              <input
                type="text"
                value={formData.firstName}
                onChange={(e) => handleInputChange('firstName', e.target.value)}
                className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.firstName ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="Digite o nome"
              />
              {errors.firstName && (
                <p className="text-xs text-red-600 mt-1">{errors.firstName}</p>
              )}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Sobrenome *
              </label>
              <input
                type="text"
                value={formData.lastName}
                onChange={(e) => handleInputChange('lastName', e.target.value)}
                className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.lastName ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="Digite o sobrenome"
              />
              {errors.lastName && (
                <p className="text-xs text-red-600 mt-1">{errors.lastName}</p>
              )}
            </div>
          </div>

          {/* Documento */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {customerType === 'pf' ? 'CPF' : 'CNPJ'}
            </label>
            {customerType === 'pf' ? (
              <input
                type="text"
                value={formData.cpf ? formatCpfInput(formData.cpf) : ''}
                onChange={(e) => handleInputChange('cpf', e.target.value.replace(/\D/g, ''))}
                className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.cpf ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="000.000.000-00"
                maxLength={14}
              />
            ) : (
              <input
                type="text"
                value={formData.cnpj ? formatCnpjInput(formData.cnpj) : ''}
                onChange={(e) => handleInputChange('cnpj', e.target.value.replace(/\D/g, ''))}
                className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.cnpj ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="00.000.000/0000-00"
                maxLength={18}
              />
            )}
            {(errors.cpf || errors.cnpj) && (
              <p className="text-xs text-red-600 mt-1">{errors.cpf || errors.cnpj}</p>
            )}
          </div>

          {/* Email e Telefone */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Email
              </label>
              <input
                type="email"
                value={formData.email}
                onChange={(e) => handleInputChange('email', e.target.value)}
                className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.email ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="email@exemplo.com"
              />
              {errors.email && (
                <p className="text-xs text-red-600 mt-1">{errors.email}</p>
              )}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Telefone
              </label>
              <input
                type="text"
                value={formData.phoneNumber ? formatPhoneInput(formData.phoneNumber) : ''}
                onChange={(e) => handleInputChange('phoneNumber', e.target.value.replace(/\D/g, ''))}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="(00) 00000-0000"
                maxLength={15}
              />
            </div>
          </div>

          {/* Endereço */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Endereço
            </label>
            <textarea
              value={formData.address}
              onChange={(e) => handleInputChange('address', e.target.value)}
              rows={3}
              className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Endereço completo..."
            />
          </div>

          {/* Actions */}
          <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
            <Button type="button" variant="outline" onClick={onClose}>
              Cancelar
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Salvando...' : customer ? 'Atualizar' : 'Criar Cliente'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
