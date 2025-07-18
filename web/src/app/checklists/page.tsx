'use client'

import { useState, useEffect, useMemo } from 'react'
import { useRouter } from 'next/navigation'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { Button } from '@/components/ui/button'
import { useChecklistsPaged } from '@/hooks/use-api'
import { CustomerService } from '@/services/customer.service'
import { CustomerType, ChecklistSearchRequest, ChecklistPagedResponse } from '@/types'
import {
  Plus,
  Search,
  Filter,
  MoreHorizontal,
  Eye,
  Edit,
  Copy,
  Archive,
  Trash2,
  Calendar,
  User,
  Building,
  FileText
} from 'lucide-react'


export default function ChecklistsPage() {
  const router = useRouter()
  const [searchInput, setSearchInput] = useState('') // Input do usuário
  const [searchTerm, setSearchTerm] = useState('') // Termo usado na API (com debounce)
  const [statusFilter, setStatusFilter] = useState('all')
  const [sortBy, setSortBy] = useState('createdDateUtc')
  const [sortDirection, setSortDirection] = useState('desc')
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const [previousData, setPreviousData] = useState<ChecklistPagedResponse | undefined>(undefined)

  // Debounce para busca - atualiza searchTerm 500ms após parar de digitar
  useEffect(() => {
    const timer = setTimeout(() => {
      setSearchTerm(searchInput)
    }, 500)

    return () => clearTimeout(timer)
  }, [searchInput])

  // Parâmetros de busca
  const searchParams: ChecklistSearchRequest = {
    search: searchTerm || undefined,
    page: currentPage,
    pageSize: pageSize,
    sortBy: sortBy,
    sortDirection: sortDirection
  }

  // Buscar checklists da API com paginação
  const { data: checklistsResponse, isLoading, error, isFetching } = useChecklistsPaged(searchParams)

  // Manter dados anteriores durante carregamento para evitar "piscar"
  useEffect(() => {
    if (checklistsResponse && !isFetching) {
      setPreviousData(checklistsResponse)
    }
  }, [checklistsResponse, isFetching])

  // Definir isRefetching primeiro
  const isRefetching = isFetching && previousData

  // Usar dados anteriores se estiver carregando e houver dados anteriores
  const displayData: ChecklistPagedResponse | undefined = isRefetching ? previousData : checklistsResponse

  const handleNewChecklist = () => {
    router.push('/checklists/new')
  }

  // Reset para primeira página quando busca ou filtros mudam (mas não pageSize)
  useEffect(() => {
    setCurrentPage(1)
  }, [searchTerm, statusFilter, sortBy, sortDirection])

  // Função para determinar status baseado nos dados do checklist
  const getChecklistStatus = (checklist: { isPublished: boolean }) => {
    if (!checklist.isPublished) return 'draft'
    // Aqui você pode adicionar mais lógica para determinar outros status
    return 'active'
  }



  const getStatusColor = (status: string) => {
    switch (status) {
      case 'active': return 'bg-blue-100 text-blue-800'
      case 'completed': return 'bg-green-100 text-green-800'
      case 'draft': return 'bg-gray-100 text-gray-800'
      case 'archived': return 'bg-yellow-100 text-yellow-800'
      default: return 'bg-gray-100 text-gray-800'
    }
  }

  const getStatusText = (status: string) => {
    switch (status) {
      case 'active': return 'Ativo'
      case 'completed': return 'Concluído'
      case 'draft': return 'Rascunho'
      case 'archived': return 'Arquivado'
      default: return status
    }
  }

  // Dados dos checklists vêm do servidor já filtrados e paginados
  const checklists = displayData?.data || []
  const totalCount = displayData?.totalCount || 0
  const totalPages = displayData?.totalPages || 0
  const hasPreviousPage = displayData?.hasPreviousPage || false
  const hasNextPage = displayData?.hasNextPage || false

  // Lógica de paginação inteligente
  // Usar dados do backend quando disponíveis, senão calcular baseado nos dados atuais

  const effectiveTotalCount = totalCount > 0 ? totalCount : checklists.length
  const effectiveTotalPages = totalPages > 0 ? totalPages : Math.ceil(effectiveTotalCount / pageSize)

  // Ajustar página atual quando pageSize muda
  useEffect(() => {
    // Quando pageSize muda, ajustar a página atual se necessário
    const newTotalPages = Math.ceil(effectiveTotalCount / pageSize)
    if (currentPage > newTotalPages && newTotalPages > 0) {
      setCurrentPage(newTotalPages)
    }
  }, [pageSize, effectiveTotalCount, currentPage])

  // Mostrar paginação quando há mais dados do que cabem em uma página
  const shouldShowPagination = effectiveTotalCount > 0 && effectiveTotalPages >= 1





  // Filtro de status é aplicado no frontend (não implementado no backend ainda)
  const filteredChecklists = checklists.filter(checklist => {
    const checklistStatus = getChecklistStatus(checklist)
    const matchesStatus = statusFilter === 'all' || checklistStatus === statusFilter
    return matchesStatus
  })

  // Funções de paginação
  const handlePreviousPage = () => {
    if (hasPreviousPage || currentPage > 1) {
      setCurrentPage(prev => prev - 1)
    }
  }

  const handleNextPage = () => {
    if (hasNextPage || currentPage < effectiveTotalPages) {
      setCurrentPage(prev => prev + 1)
    }
  }

  const handleSortChange = (newSortBy: string) => {
    if (sortBy === newSortBy) {
      // Se já está ordenando por este campo, inverte a direção
      setSortDirection(prev => prev === 'asc' ? 'desc' : 'asc')
    } else {
      // Novo campo, usa direção padrão
      setSortBy(newSortBy)
      setSortDirection('desc')
    }
  }

  // Função para gerar números de páginas para navegação
  // Função para gerar números de páginas para navegação (memoizada para performance)
  const pageNumbers = useMemo((): number[] => {
    const pages: number[] = []
    const maxVisiblePages = 5

    // Se há apenas 1 página ou menos, não mostrar números
    if (effectiveTotalPages <= 1) {
      pages.push(1);
      return pages
    }

    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2))
    let endPage = Math.min(effectiveTotalPages, startPage + maxVisiblePages - 1)

    // Ajustar startPage se estivermos próximos ao final
    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1)
      endPage = Math.min(effectiveTotalPages, startPage + maxVisiblePages - 1)
    }

    console.log("startPage: ", startPage, "endPage: ", endPage)

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i)
    }

    return pages
  }, [effectiveTotalPages, currentPage])

  const handlePageClick = (page: number) => {
    setCurrentPage(page)
  }

  // Distinguir entre carregamento inicial e refetch
  const isInitialLoading = isLoading && !previousData

  // Loading state inicial (primeira vez)
  if (isInitialLoading) {
    return (
      <DashboardLayout>
        <div className="p-8">
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
              <p className="mt-2 text-gray-600">Carregando checklists...</p>
            </div>
          </div>
        </div>
      </DashboardLayout>
    )
  }

  // Error state
  if (error) {
    return (
      <DashboardLayout>
        <div className="p-8">
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <p className="text-red-600">Erro ao carregar checklists</p>
              <p className="text-gray-600 mt-1">Tente recarregar a página</p>
            </div>
          </div>
        </div>
      </DashboardLayout>
    )
  }

  return (
    <DashboardLayout>
      <div className="p-8">
        <div className="space-y-6">
          {/* Header */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="h-12 w-12 bg-blue-100 rounded-full flex items-center justify-center">
                  <FileText className="h-6 w-6 text-blue-600" />
                </div>
                <div>
                  <h1 className="text-2xl font-bold text-gray-900">Checklists</h1>
                  <p className="text-gray-600">Gerencie todos os seus checklists de onboarding</p>
                </div>
              </div>
              <Button
                className="bg-blue-600 hover:bg-blue-700"
                onClick={handleNewChecklist}
              >
                <Plus className="h-4 w-4 mr-2" />
                Novo Checklist
              </Button>
            </div>
          </div>
          {/* Filters and Search */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="h-4 w-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar por título ou cliente..."
                  value={searchInput}
                  onChange={(e) => setSearchInput(e.target.value)}
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>
            
            <div className="flex gap-2">
              <select
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="all">Todos os Status</option>
                <option value="draft">Rascunho</option>
                <option value="active">Ativo</option>
                <option value="completed">Concluído</option>
                <option value="archived">Arquivado</option>
              </select>
              
              <select
                value={sortBy}
                onChange={(e) => handleSortChange(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="createdDateUtc">Mais Recentes</option>
                <option value="title">Título A-Z</option>
                <option value="deadline">Prazo</option>
                <option value="lastUpdatedDateUtc">Última Atualização</option>
              </select>

              <select
                value={pageSize}
                onChange={(e) => setPageSize(Number(e.target.value))}
                className="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value={10}>10 por página</option>
                <option value={20}>20 por página</option>
                <option value={30}>30 por página</option>
                <option value={50}>50 por página</option>
                <option value={100}>100 por página</option>
              </select>

              <Button variant="outline">
                <Filter className="h-4 w-4 mr-2" />
                Filtros
              </Button>
            </div>
          </div>
        </div>

          {/* Checklists Table */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden relative">

            <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Checklist
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Cliente
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Progresso
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Prazo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Última Atividade
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider w-32">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredChecklists.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center">
                      <div className="text-gray-500">
                        <FileText className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                        <div className="flex items-center justify-center mb-2">
                          <p className="text-lg font-medium text-gray-900">
                            Nenhum checklist encontrado
                          </p>
                          {isRefetching && (
                            <div className="ml-2 animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
                          )}
                        </div>
                        <p className="text-sm">
                          {searchTerm
                            ? `Nenhum registro para o termo de pesquisa "${searchTerm}" foi encontrado.`
                            : 'Não há checklists para exibir com os filtros selecionados.'
                          }
                        </p>
                      </div>
                    </td>
                  </tr>
                ) : (
                  filteredChecklists.map((checklist) => (
                    <tr key={checklist.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div>
                          <div className="text-sm font-medium text-gray-900">{checklist.title}</div>
                          <div className="text-sm text-gray-500">
                            {checklist.sections?.length || 0} seções
                          </div>
                        </div>
                      </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        {checklist.customer ? (
                          checklist.customer.type === CustomerType.LegalCompany ? (
                            <Building className="h-4 w-4 text-gray-400 mr-2" />
                          ) : (
                            <User className="h-4 w-4 text-gray-400 mr-2" />
                          )
                        ) : (
                          <User className="h-4 w-4 text-gray-400 mr-2" />
                        )}
                        <span className="text-sm text-gray-900">
                          {checklist.customer
                            ? CustomerService.getDisplayName(checklist.customer)
                            : 'Cliente não definido'
                          }
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(getChecklistStatus(checklist))}`}>
                        {getStatusText(getChecklistStatus(checklist))}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="w-16 bg-gray-200 rounded-full h-2 mr-2">
                          <div
                            className="bg-blue-600 h-2 rounded-full"
                            style={{ width: '0%' }}
                          />
                        </div>
                        <span className="text-sm text-gray-900">0%</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <Calendar className="h-4 w-4 text-gray-400 mr-2" />
                        <span className="text-sm text-gray-900">
                          {checklist.deadline
                            ? new Date(checklist.deadline).toLocaleDateString('pt-BR')
                            : 'Sem prazo'
                          }
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {new Date(checklist.lastUpdatedDateUtc || checklist.createdDateUtc).toLocaleDateString('pt-BR')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => router.push(`/checklists/${checklist.id}`)}
                          title="Visualizar"
                        >
                          <Eye className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => router.push(`/checklists/${checklist.id}/edit`)}
                          title="Editar"
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button variant="ghost" size="sm" title="Mais opções">
                          <MoreHorizontal className="h-4 w-4" />
                        </Button>
                      </div>
                    </td>
                  </tr>
                  ))
                )}
              </tbody>
            </table>
            </div>
          </div>

          {/* Pagination */}
          {shouldShowPagination && (
            <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
              <div className="text-sm text-gray-700">
                Mostrando <span className="font-medium">{((currentPage - 1) * pageSize) + 1}</span> a <span className="font-medium">{Math.min(currentPage * pageSize, effectiveTotalCount)}</span> de{' '}
                <span className="font-medium">{effectiveTotalCount}</span> resultados
              </div>

              <div className="flex items-center space-x-1">
                {/* Botão Anterior */}
                <Button
                  variant="outline"
                  size="sm"
                  disabled={!(hasPreviousPage || currentPage > 1) || isInitialLoading}
                  onClick={handlePreviousPage}
                  className="px-3 py-1"
                >
                  Anterior
                </Button>

                {/* Números das páginas */}
                <div className="flex items-center space-x-1">
                  {pageNumbers.map((pageNum) => (
                    <Button
                      key={pageNum}
                      variant={pageNum === currentPage ? "default" : "outline"}
                      size="sm"
                      onClick={() => handlePageClick(pageNum)}
                      disabled={isInitialLoading}
                      className="w-8 h-8 p-0"
                    >
                      {pageNum}
                    </Button>
                  ))}

                  {/* Mostrar página atual se não há números */}
                  {effectiveTotalPages > 1 && pageNumbers.length === 0 && (
                    <span className="text-sm text-gray-600 px-2">
                      Página {currentPage} de {effectiveTotalPages}
                    </span>
                  )}
                </div>

                {/* Botão Próximo */}
                <Button
                  variant="outline"
                  size="sm"
                  disabled={!(hasNextPage || currentPage < effectiveTotalPages) || isInitialLoading}
                  onClick={handleNextPage}
                  className="px-3 py-1"
                >
                  Próximo
                </Button>
              </div>
            </div>
          )}
      </div>
      </div>
    </DashboardLayout>
  )
}
