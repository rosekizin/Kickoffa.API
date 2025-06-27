'use client'

import { useState } from 'react'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { Button } from '@/components/ui/button'
import { CustomerService } from '@/services/customer.service'
import { CustomerModal } from '@/components/customers/customer-modal'
import { useCustomers, useDeleteCustomer } from '@/hooks/use-api'
import { Customer } from '@/types'
import {
  Plus,
  Search,
  Filter,
  MoreHorizontal,
  Edit,
  Trash2,
  User,
  Building,
  Phone,
  Mail,
  MapPin
} from 'lucide-react'

export default function CustomersPage() {
  const [searchTerm, setSearchTerm] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [editingCustomer, setEditingCustomer] = useState<Customer | undefined>()

  // Hooks do TanStack Query
  const { data: customers = [], isLoading: loading, error } = useCustomers()
  const deleteCustomerMutation = useDeleteCustomer()

  const filteredCustomers = customers.filter(customer => {
    const searchLower = searchTerm.toLowerCase()
    return (
      customer.firstName.toLowerCase().includes(searchLower) ||
      customer.lastName.toLowerCase().includes(searchLower) ||
      customer.email?.toLowerCase().includes(searchLower) ||
      customer.cpf?.includes(searchTerm) ||
      customer.cnpj?.includes(searchTerm)
    )
  })

  const handleDeleteCustomer = async (id: string) => {
    if (!confirm('Tem certeza que deseja excluir este cliente?')) return

    try {
      await deleteCustomerMutation.mutateAsync(id)
      // TODO: Mostrar toast de sucesso
    } catch (error) {
      console.error('Erro ao excluir cliente:', error)
      // TODO: Mostrar toast de erro
    }
  }

  const handleSaveCustomer = (customer: Customer) => {
    // O cache será atualizado automaticamente pelos hooks
    setShowModal(false)
    setEditingCustomer(undefined)
  }

  const handleEditCustomer = (customer: Customer) => {
    setEditingCustomer(customer)
    setShowModal(true)
  }

  return (
    <DashboardLayout>
      <div className="flex flex-col h-full">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Clientes</h1>
            <p className="text-gray-600">Gerencie seus clientes e informações de contato</p>
          </div>
          <Button onClick={() => setShowModal(true)}>
            <Plus className="h-4 w-4 mr-2" />
            Novo Cliente
          </Button>
        </div>

        {/* Filters */}
        <div className="flex items-center space-x-4 mb-6">
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              type="text"
              placeholder="Buscar clientes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <Button variant="outline">
            <Filter className="h-4 w-4 mr-2" />
            Filtros
          </Button>
        </div>

        {/* Content */}
        <div className="flex-1 bg-white rounded-lg border border-gray-200">
          {loading ? (
            <div className="flex items-center justify-center h-64">
              <div className="text-center">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-4"></div>
                <p className="text-gray-600">Carregando clientes...</p>
              </div>
            </div>
          ) : filteredCustomers.length === 0 ? (
            <div className="flex items-center justify-center h-64">
              <div className="text-center">
                <User className="h-16 w-16 text-gray-300 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-gray-900 mb-2">
                  {searchTerm ? 'Nenhum cliente encontrado' : 'Nenhum cliente cadastrado'}
                </h3>
                <p className="text-gray-600 mb-4">
                  {searchTerm 
                    ? 'Tente ajustar os termos de busca'
                    : 'Comece criando seu primeiro cliente'
                  }
                </p>
                {!searchTerm && (
                  <Button onClick={() => setShowModal(true)}>
                    <Plus className="h-4 w-4 mr-2" />
                    Criar Primeiro Cliente
                  </Button>
                )}
              </div>
            </div>
          ) : (
            <div className="overflow-hidden">
              {/* Table Header */}
              <div className="bg-gray-50 px-6 py-3 border-b border-gray-200">
                <div className="grid grid-cols-12 gap-4 text-xs font-medium text-gray-500 uppercase tracking-wider">
                  <div className="col-span-3">Cliente</div>
                  <div className="col-span-2">Documento</div>
                  <div className="col-span-2">Contato</div>
                  <div className="col-span-3">Endereço</div>
                  <div className="col-span-2">Ações</div>
                </div>
              </div>

              {/* Table Body */}
              <div className="divide-y divide-gray-200">
                {filteredCustomers.map((customer) => (
                  <div key={customer.id} className="px-6 py-4 hover:bg-gray-50">
                    <div className="grid grid-cols-12 gap-4 items-center">
                      {/* Cliente */}
                      <div className="col-span-3">
                        <div className="flex items-center">
                          <div className="h-10 w-10 bg-blue-100 rounded-full flex items-center justify-center">
                            {customer.cnpj ? (
                              <Building className="h-5 w-5 text-blue-600" />
                            ) : (
                              <User className="h-5 w-5 text-blue-600" />
                            )}
                          </div>
                          <div className="ml-3">
                            <p className="text-sm font-medium text-gray-900">
                              {customer.firstName} {customer.lastName}
                            </p>
                            <p className="text-sm text-gray-500">
                              {customer.cnpj ? 'Pessoa Jurídica' : 'Pessoa Física'}
                            </p>
                          </div>
                        </div>
                      </div>

                      {/* Documento */}
                      <div className="col-span-2">
                        <p className="text-sm text-gray-900">
                          {customer.cpf && CustomerService.formatCpf(customer.cpf)}
                          {customer.cnpj && CustomerService.formatCnpj(customer.cnpj)}
                        </p>
                      </div>

                      {/* Contato */}
                      <div className="col-span-2">
                        <div className="space-y-1">
                          {customer.email && (
                            <div className="flex items-center text-sm text-gray-600">
                              <Mail className="h-3 w-3 mr-1" />
                              {customer.email}
                            </div>
                          )}
                          {customer.phoneNumber && (
                            <div className="flex items-center text-sm text-gray-600">
                              <Phone className="h-3 w-3 mr-1" />
                              {CustomerService.formatPhone(customer.phoneNumber)}
                            </div>
                          )}
                        </div>
                      </div>

                      {/* Endereço */}
                      <div className="col-span-3">
                        {customer.address ? (
                          <div className="flex items-center text-sm text-gray-600">
                            <MapPin className="h-3 w-3 mr-1 flex-shrink-0" />
                            <span className="truncate">{customer.address}</span>
                          </div>
                        ) : (
                          <span className="text-sm text-gray-400">-</span>
                        )}
                      </div>

                      {/* Ações */}
                      <div className="col-span-2">
                        <div className="flex items-center space-x-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleEditCustomer(customer)}
                            className="text-blue-600 hover:text-blue-700"
                          >
                            <Edit className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDeleteCustomer(customer.id)}
                            className="text-red-600 hover:text-red-700"
                            disabled={deleteCustomerMutation.isPending}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                          <Button variant="ghost" size="sm">
                            <MoreHorizontal className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Stats */}
        <div className="mt-6 flex items-center justify-between text-sm text-gray-600">
          <p>
            Mostrando {filteredCustomers.length} de {customers.length} clientes
          </p>
          <div className="flex items-center space-x-4">
            <span>Total: {customers.length} clientes</span>
            <span>•</span>
            <span>PF: {customers.filter(c => c.cpf).length}</span>
            <span>•</span>
            <span>PJ: {customers.filter(c => c.cnpj).length}</span>
          </div>
        </div>
      </div>

      {/* Modal de criação/edição */}
      <CustomerModal
        isOpen={showModal}
        onClose={() => {
          setShowModal(false)
          setEditingCustomer(undefined)
        }}
        onSave={handleSaveCustomer}
        customer={editingCustomer}
      />
    </DashboardLayout>
  )
}
