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

  const handleDeleteCustomer = async (id: number) => {
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
    <DashboardLayout showSearchBar={false}>
      <div className="p-8">
        <div className="space-y-6">
          {/* Header */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="h-12 w-12 bg-blue-100 rounded-full flex items-center justify-center">
                  <User className="h-6 w-6 text-blue-600" />
                </div>
                <div>
                  <h1 className="text-2xl font-bold text-gray-900">Clientes</h1>
                  <p className="text-gray-600">Gerencie seus clientes e informações de contato</p>
                </div>
              </div>
              <Button onClick={() => setShowModal(true)} className="bg-blue-600 hover:bg-blue-700">
                <Plus className="h-4 w-4 mr-2" />
                Novo Cliente
              </Button>
            </div>
          </div>

          {/* Filters */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center space-x-4">
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
          </div>

          {/* Content */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
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
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Cliente
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Documento
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Contato
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Endereço
                    </th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Ações
                    </th>
                  </tr>
                </thead>

                <tbody className="bg-white divide-y divide-gray-200">
                  {filteredCustomers.map((customer) => (
                    <tr key={customer.id} className="hover:bg-gray-50">
                      {/* Cliente */}
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="h-10 w-10 bg-blue-100 rounded-full flex items-center justify-center">
                            {customer.cnpj ? (
                              <Building className="h-5 w-5 text-blue-600" />
                            ) : (
                              <User className="h-5 w-5 text-blue-600" />
                            )}
                          </div>
                          <div className="ml-3">
                            <div className="text-sm font-medium text-gray-900">
                              {customer.firstName} {customer.lastName}
                            </div>
                            <div className="text-sm text-gray-500">
                              {customer.cnpj ? 'Pessoa Jurídica' : 'Pessoa Física'}
                            </div>
                          </div>
                        </div>
                      </td>

                      {/* Documento */}
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="text-sm text-gray-900">
                          {customer.cpf && CustomerService.formatCpf(customer.cpf)}
                          {customer.cnpj && CustomerService.formatCnpj(customer.cnpj)}
                        </span>
                      </td>

                      {/* Contato */}
                      <td className="px-6 py-4 whitespace-nowrap">
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
                      </td>

                      {/* Endereço */}
                      <td className="px-6 py-4 whitespace-nowrap">
                        {customer.address ? (
                          <div className="flex items-center text-sm text-gray-600">
                            <MapPin className="h-3 w-3 mr-1 flex-shrink-0" />
                            <span className="truncate">{customer.address}</span>
                          </div>
                        ) : (
                          <span className="text-sm text-gray-400">-</span>
                        )}
                      </td>

                      {/* Ações */}
                      <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                        <div className="flex items-center justify-end space-x-2">
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
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          </div>

          {/* Stats */}
          <div className="flex items-center justify-between mt-6">
            <div className="text-sm text-gray-700">
              Mostrando <span className="font-medium">1</span> a <span className="font-medium">{filteredCustomers.length}</span> de{' '}
              <span className="font-medium">{customers.length}</span> resultados
            </div>
            <div className="flex items-center space-x-4 text-sm text-gray-600">
              <span>Total: {customers.length} clientes</span>
              <span>•</span>
              <span>PF: {customers.filter(c => c.cpf).length}</span>
              <span>•</span>
              <span>PJ: {customers.filter(c => c.cnpj).length}</span>
            </div>
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
