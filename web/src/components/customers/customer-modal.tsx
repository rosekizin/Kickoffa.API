'use client'

import { useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import { CustomerService } from '@/services/customer.service'
import { useCreateCustomer, useUpdateCustomer } from '@/hooks/use-api'
import { CustomerUnion, CreateCustomerRequestUnion, CreateNaturalPersonRequest, CreateLegalPersonRequest, CustomerType } from '@/types'
import { X, User, Building, AlertCircle } from 'lucide-react'

interface CustomerModalProps {
  isOpen: boolean
  onClose: () => void
  onSave: (customer: CustomerUnion) => void
  customer?: CustomerUnion
}

export function CustomerModal({ isOpen, onClose, onSave, customer }: CustomerModalProps) {
  const [customerType, setCustomerType] = useState<CustomerType>(CustomerType.NaturalPerson)
  const [formData, setFormData] = useState({
    // Campos comuns
    email: '',
    phoneNumber: '',
    address: '',
    // Campos para pessoa física
    firstName: '',
    lastName: '',
    cpf: '',
    // Campos para pessoa jurídica
    company: '',
    cnpj: ''
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  // Hooks do TanStack Query
  const createCustomerMutation = useCreateCustomer()
  const updateCustomerMutation = useUpdateCustomer()

  useEffect(() => {
    if (customer) {
      setCustomerType(customer.type)

      if (CustomerService.isNaturalPerson(customer)) {
        setFormData({
          email: customer.email || '',
          phoneNumber: customer.phoneNumber || '',
          address: customer.address || '',
          firstName: customer.firstName,
          lastName: customer.lastName,
          cpf: customer.cpf || '',
          company: '',
          cnpj: ''
        })
      } else if (CustomerService.isLegalPerson(customer)) {
        setFormData({
          email: customer.email || '',
          phoneNumber: customer.phoneNumber || '',
          address: customer.address || '',
          firstName: '',
          lastName: '',
          cpf: '',
          company: customer.company,
          cnpj: customer.cnpj || ''
        })
      }
    } else {
      setCustomerType(CustomerType.NaturalPerson)
      setFormData({
        email: '',
        phoneNumber: '',
        address: '',
        firstName: '',
        lastName: '',
        cpf: '',
        company: '',
        cnpj: ''
      })
    }
    setErrors({})
  }, [customer, isOpen])

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {}

    // Validações comuns
    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Email inválido'
    }

    // Validações específicas por tipo
    if (customerType === CustomerType.NaturalPerson) {
      if (!formData.firstName.trim()) {
        newErrors.firstName = 'Nome é obrigatório'
      }

      if (!formData.lastName.trim()) {
        newErrors.lastName = 'Sobrenome é obrigatório'
      }

      if (formData.cpf && !CustomerService.validateCpf(formData.cpf)) {
        newErrors.cpf = 'CPF inválido'
      }
    } else if (customerType === CustomerType.LegalCompany) {
      if (!formData.company.trim()) {
        newErrors.company = 'Nome da empresa é obrigatório'
      }

      if (formData.cnpj && !CustomerService.validateCnpj(formData.cnpj)) {
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
      let dataToSend: CreateCustomerRequestUnion

      if (customerType === CustomerType.NaturalPerson) {
        dataToSend = {
          type: CustomerType.NaturalPerson,
          firstName: formData.firstName.trim(),
          lastName: formData.lastName.trim(),
          cpf: formData.cpf?.replace(/\D/g, '') || undefined,
          email: formData.email?.trim() || undefined,
          phoneNumber: formData.phoneNumber?.trim() || undefined,
          address: formData.address?.trim() || undefined
        } as CreateNaturalPersonRequest
      } else {
        dataToSend = {
          type: CustomerType.LegalCompany,
          company: formData.company.trim(),
          cnpj: formData.cnpj?.replace(/\D/g, '') || undefined,
          email: formData.email?.trim() || undefined,
          phoneNumber: formData.phoneNumber?.trim() || undefined,
          address: formData.address?.trim() || undefined
        } as CreateLegalPersonRequest
      }

      console.log('Enviando dados para API:', dataToSend)

      let savedCustomer: CustomerUnion
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

  const handleInputChange = (field: string, value: string) => {
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
                onClick={() => !customer && setCustomerType(CustomerType.NaturalPerson)}
                disabled={!!customer}
                className={`p-4 border rounded-lg flex items-center justify-center space-x-2 transition-colors ${
                  customerType === CustomerType.NaturalPerson
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : customer
                    ? 'border-gray-200 bg-gray-50 text-gray-400 cursor-not-allowed'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
              >
                <User className="h-5 w-5" />
                <span>Pessoa Física</span>
              </button>
              <button
                type="button"
                onClick={() => !customer && setCustomerType(CustomerType.LegalCompany)}
                disabled={!!customer}
                className={`p-4 border rounded-lg flex items-center justify-center space-x-2 transition-colors ${
                  customerType === CustomerType.LegalCompany
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : customer
                    ? 'border-gray-200 bg-gray-50 text-gray-400 cursor-not-allowed'
                    : 'border-gray-300 hover:border-gray-400'
                }`}
              >
                <Building className="h-5 w-5" />
                <span>Pessoa Jurídica</span>
              </button>
            </div>
            {customer && (
              <p className="text-xs text-gray-500 mt-2">
                O tipo de cliente não pode ser alterado após a criação
              </p>
            )}
          </div>

          {/* Campos específicos por tipo */}
          {customerType === CustomerType.NaturalPerson ? (
            // Campos para Pessoa Física
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
          ) : (
            // Campos para Pessoa Jurídica
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Nome da Empresa *
              </label>
              <input
                type="text"
                value={formData.company}
                onChange={(e) => handleInputChange('company', e.target.value)}
                className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.company ? 'border-red-300' : 'border-gray-300'
                }`}
                placeholder="Digite o nome da empresa"
              />
              {errors.company && (
                <p className="text-xs text-red-600 mt-1">{errors.company}</p>
              )}
            </div>
          )}

          {/* Documento */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              {customerType === CustomerType.NaturalPerson ? 'CPF' : 'CNPJ'}
              {customer && (
                <span className="text-xs text-gray-500 ml-1">(não editável)</span>
              )}
            </label>
            {customerType === CustomerType.NaturalPerson ? (
              <input
                type="text"
                value={formData.cpf ? formatCpfInput(formData.cpf) : ''}
                onChange={(e) => handleInputChange('cpf', e.target.value.replace(/\D/g, ''))}
                readOnly={!!customer}
                disabled={!!customer}
                className={`w-full px-3 py-2 border rounded-md text-sm ${
                  customer
                    ? 'bg-gray-100 cursor-not-allowed text-gray-600'
                    : 'focus:outline-none focus:ring-2 focus:ring-blue-500'
                } ${
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
                readOnly={!!customer}
                disabled={!!customer}
                className={`w-full px-3 py-2 border rounded-md text-sm ${
                  customer
                    ? 'bg-gray-100 cursor-not-allowed text-gray-600'
                    : 'focus:outline-none focus:ring-2 focus:ring-blue-500'
                } ${
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
