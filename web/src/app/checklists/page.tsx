'use client'

import { useState, useEffect, useMemo } from 'react'
import { useRouter } from 'next/navigation'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { Button } from '@/components/ui/button'
import { StatusDropdown } from '@/components/ui/status-dropdown'
import { PageSizeDropdown } from '@/components/ui/page-size-dropdown'
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
  Calendar,
  User,
  Building,
  FileText,
  ChevronUp,
  ChevronDown,
  ChevronsUpDown
} from 'lucide-react'


export default function ChecklistsPage() {
  const router = useRouter()
  const [searchInput, setSearchInput] = useState('') // Input do usuário
  const [searchTerm, setSearchTerm] = useState('') // Termo usado na API (com debounce)
  const [statusFilter, setStatusFilter] = useState('all')
  const [sortBy, setSortBy] = useState<'title' | 'customer' | 'status' | 'progress' | 'deadline' | 'lastUpdatedDateUtc'>('lastUpdatedDateUtc')
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc')
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

  // Parâmetros de busca (sem ordenação - será feita no frontend)
  const searchParams: ChecklistSearchRequest = {
    search: searchTerm || undefined,
    page: currentPage,
    pageSize: pageSize
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

  // Reset para primeira página quando busca ou filtros mudam (não incluir ordenação)
  useEffect(() => {
    setCurrentPage(1)
  }, [searchTerm, statusFilter])

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

  // Ordenação aplicada no frontend
  const sortedChecklists = useMemo(() => {
    const sorted = [...filteredChecklists].sort((a, b) => {
      let aValue: string | number
      let bValue: string | number

      switch (sortBy) {
        case 'title':
          aValue = a.title.toLowerCase()
          bValue = b.title.toLowerCase()
          break
        case 'customer':
          aValue = a.customer ? CustomerService.getDisplayName(a.customer).toLowerCase() : ''
          bValue = b.customer ? CustomerService.getDisplayName(b.customer).toLowerCase() : ''
          break
        case 'status':
          // Definir ordem lógica dos status: draft (0) -> active (1) -> completed (2) -> archived (3)
          const getStatusOrder = (status: string) => {
            switch (status) {
              case 'draft': return 0
              case 'active': return 1
              case 'completed': return 2
              case 'archived': return 3
              default: return 4
            }
          }
          aValue = getStatusOrder(getChecklistStatus(a))
          bValue = getStatusOrder(getChecklistStatus(b))
          break
        case 'progress':
          // Por enquanto, progresso é sempre 0%, mas preparado para futuras implementações
          aValue = 0
          bValue = 0
          break
        case 'deadline':
          aValue = a.deadline ? new Date(a.deadline).getTime() : 0
          bValue = b.deadline ? new Date(b.deadline).getTime() : 0
          break
        case 'lastUpdatedDateUtc':
        default:
          aValue = new Date(a.lastUpdatedDateUtc || a.createdDateUtc).getTime()
          bValue = new Date(b.lastUpdatedDateUtc || b.createdDateUtc).getTime()
          break
      }

      if (aValue < bValue) return sortDirection === 'asc' ? -1 : 1
      if (aValue > bValue) return sortDirection === 'asc' ? 1 : -1
      return 0
    })

    return sorted
  }, [filteredChecklists, sortBy, sortDirection])

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

  const handleSortChange = (newSortBy: 'title' | 'customer' | 'status' | 'progress' | 'deadline' | 'lastUpdatedDateUtc') => {
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

  // Componente para cabeçalho ordenável
  const SortableHeader = ({
    field,
    children,
    className = ""
  }: {
    field: 'title' | 'customer' | 'status' | 'progress' | 'deadline' | 'lastUpdatedDateUtc'
    children: React.ReactNode
    className?: string
  }) => {
    const isActive = sortBy === field
    const isAsc = isActive && sortDirection === 'asc'

    return (
      <th
        className={`px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100 transition-colors ${className}`}
        onClick={() => handleSortChange(field)}
      >
        <div className="flex items-center space-x-1">
          <span>{children}</span>
          <div className="flex flex-col">
            {isActive ? (
              isAsc ? (
                <ChevronUp className="h-3 w-3 text-gray-700" />
              ) : (
                <ChevronDown className="h-3 w-3 text-gray-700" />
              )
            ) : (
              <ChevronsUpDown className="h-3 w-3 text-gray-400" />
            )}
          </div>
        </div>
      </th>
    )
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
              <StatusDropdown
                value={statusFilter}
                onChange={setStatusFilter}
                className="min-w-[180px]"
              />

              <PageSizeDropdown
                value={pageSize}
                onChange={setPageSize}
                className="min-w-[160px]"
              />

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
                  <SortableHeader field="title">
                    Checklist
                  </SortableHeader>
                  <SortableHeader field="customer">
                    Cliente
                  </SortableHeader>
                  <SortableHeader field="status">
                    Status
                  </SortableHeader>
                  <SortableHeader field="progress">
                    Progresso
                  </SortableHeader>
                  <SortableHeader field="deadline">
                    Prazo
                  </SortableHeader>
                  <SortableHeader field="lastUpdatedDateUtc">
                    Última Atividade
                  </SortableHeader>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider w-32">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {sortedChecklists.length === 0 ? (
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
                  sortedChecklists.map((checklist) => (
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
