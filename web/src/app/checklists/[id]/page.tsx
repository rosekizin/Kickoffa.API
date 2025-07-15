'use client'

import { useParams, useRouter } from 'next/navigation'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { useChecklist } from '@/hooks/use-api'
import { useApiError } from '@/hooks/use-api-error'
import { CustomerService } from '@/services/customer.service'
import { Button } from '@/components/ui/button'
import { 
  ArrowLeft, 
  Edit3, 
  Share2, 
  Eye, 
  Settings,
  Calendar,
  Clock,
  User,
  FileText,
  CheckSquare
} from 'lucide-react'
import { formatDistanceToNow } from 'date-fns'
import { ptBR } from 'date-fns/locale'

export default function ChecklistDetailPage() {
  const params = useParams()
  const router = useRouter()
  const { handleApiError } = useApiError()
  
  const checklistId = params.id as string
  const { data: checklist, isLoading, error } = useChecklist(checklistId)

  // Tratar erro da API
  if (error) {
    handleApiError(error, 'Erro ao carregar checklist')
  }

  if (isLoading) {
    return (
      <DashboardLayout showSearchBar={false}>
        <div className="p-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-200 rounded w-1/3 mb-4"></div>
            <div className="h-4 bg-gray-200 rounded w-1/2 mb-8"></div>
            <div className="space-y-4">
              <div className="h-32 bg-gray-200 rounded"></div>
              <div className="h-32 bg-gray-200 rounded"></div>
            </div>
          </div>
        </div>
      </DashboardLayout>
    )
  }

  if (!checklist) {
    return (
      <DashboardLayout showSearchBar={false}>
        <div className="p-8">
          <div className="text-center py-12">
            <FileText className="h-16 w-16 mx-auto mb-4 text-gray-300" />
            <h2 className="text-xl font-semibold text-gray-900 mb-2">
              Checklist não encontrado
            </h2>
            <p className="text-gray-600 mb-6">
              O checklist que você está procurando não existe ou foi removido.
            </p>
            <Button onClick={() => router.push('/checklists')}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Voltar para Checklists
            </Button>
          </div>
        </div>
      </DashboardLayout>
    )
  }

  const handleEdit = () => {
    router.push(`/checklists/${checklist.id}/edit`)
  }

  const handlePublish = () => {
    // TODO: Implementar publicação
    console.log('Publicar checklist:', checklist.id)
  }

  const handleViewPublic = () => {
    // TODO: Abrir visualização pública
    console.log('Ver versão pública:', checklist.accessToken)
  }

  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString)
      return formatDistanceToNow(date, { addSuffix: true, locale: ptBR })
    } catch {
      return dateString
    }
  }

  const totalComponents = checklist.sections.reduce((acc, section) => 
    acc + (section.components?.length || 0), 0
  )

  return (
    <DashboardLayout showSearchBar={false}>
      <div className="p-8">
        <div className="space-y-6">
          {/* Header */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-4">
              <Button
                variant="ghost"
                size="sm"
                onClick={() => router.push('/checklists')}
                className="text-gray-600 hover:text-gray-900"
              >
                <ArrowLeft className="h-4 w-4 mr-2" />
                Voltar
              </Button>

              <div className="flex items-center space-x-2">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={handleEdit}
                  className="text-gray-600 hover:text-gray-900"
                >
                  <Edit3 className="h-4 w-4 mr-2" />
                  Editar
                </Button>

                {checklist.isPublished ? (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={handleViewPublic}
                    className="text-blue-600 hover:text-blue-700"
                  >
                    <Eye className="h-4 w-4 mr-2" />
                    Ver Público
                  </Button>
                ) : (
                  <Button
                    size="sm"
                    onClick={handlePublish}
                    className="bg-blue-600 hover:bg-blue-700 text-white"
                  >
                    <Share2 className="h-4 w-4 mr-2" />
                    Publicar
                  </Button>
                )}

                <Button
                  variant="ghost"
                  size="sm"
                  className="text-gray-600 hover:text-gray-900"
                >
                  <Settings className="h-4 w-4" />
                </Button>
              </div>
            </div>

            <div className="flex items-start space-x-4">
              <div className="h-16 w-16 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                <FileText className="h-8 w-8 text-blue-600" />
              </div>
              
              <div className="flex-1 min-w-0">
                <h1 className="text-3xl font-bold text-gray-900 mb-2">
                  {checklist.title}
                </h1>
                
                {checklist.description && (
                  <p className="text-lg text-gray-600 mb-4">
                    {checklist.description}
                  </p>
                )}

                <div className="flex flex-wrap items-center gap-4 text-sm text-gray-500">
                  <div className="flex items-center">
                    <User className="h-4 w-4 mr-1" />
                    Cliente: {checklist.customer
                      ? CustomerService.getDisplayName(checklist.customer)
                      : 'Não definido'
                    }
                  </div>

                  <div className="flex items-center">
                    <CheckSquare className="h-4 w-4 mr-1" />
                    {checklist.sections.length} seç{checklist.sections.length !== 1 ? 'ões' : 'ão'}
                  </div>
                  
                  <div className="flex items-center">
                    <FileText className="h-4 w-4 mr-1" />
                    {totalComponents} component{totalComponents !== 1 ? 'es' : 'e'}
                  </div>

                  {checklist.deadline && (
                    <div className="flex items-center">
                      <Calendar className="h-4 w-4 mr-1" />
                      Prazo: {new Date(checklist.deadline).toLocaleDateString('pt-BR')}
                    </div>
                  )}

                  <div className="flex items-center">
                    <Clock className="h-4 w-4 mr-1" />
                    Criado {formatDate(checklist.createdAt)}
                  </div>

                  <div className="flex items-center">
                    <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                      checklist.isPublished 
                        ? 'bg-green-100 text-green-800' 
                        : 'bg-yellow-100 text-yellow-800'
                    }`}>
                      {checklist.isPublished ? 'Publicado' : 'Rascunho'}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Sections */}
          <div className="space-y-6">
            {checklist.sections.map((section) => (
              <div key={section.id} className="bg-white rounded-lg shadow-sm border border-gray-200">
                <div className="px-6 py-4 border-b border-gray-200">
                  <div className="flex items-center justify-between">
                    <h2 className="text-xl font-semibold text-gray-900 flex items-center">
                      {section.type === 'briefing' ? (
                        <FileText className="h-5 w-5 mr-2 text-blue-600" />
                      ) : (
                        <CheckSquare className="h-5 w-5 mr-2 text-green-600" />
                      )}
                      {section.title}
                    </h2>
                    <span className="text-sm text-gray-500">
                      {section.type === 'briefing' ? 'Briefing' : 'Checklist'}
                    </span>
                  </div>
                </div>

                <div className="p-6">
                  {section.type === 'briefing' && section.contentHtml && (
                    <div
                      className="prose max-w-none"
                      dangerouslySetInnerHTML={{ __html: section.contentHtml }}
                    />
                  )}

                  {section.type === 'checklist' && (
                    <div className="space-y-4">
                      {section.components?.length ? (
                        section.components.map((component) => (
                          <div key={component.id} className="border border-gray-200 rounded-lg p-4">
                            <div className="flex items-start justify-between">
                              <div className="flex-1">
                                <h3 className="font-medium text-gray-900 mb-1">
                                  {component.title}
                                  {component.isRequired && (
                                    <span className="text-red-500 ml-1">*</span>
                                  )}
                                </h3>
                                {component.description && (
                                  <p className="text-sm text-gray-600 mb-2">
                                    {component.description}
                                  </p>
                                )}
                                <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                                  {component.type}
                                </span>
                              </div>
                            </div>
                          </div>
                        ))
                      ) : (
                        <p className="text-gray-500 italic">Nenhum componente adicionado ainda</p>
                      )}
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </DashboardLayout>
  )
}
