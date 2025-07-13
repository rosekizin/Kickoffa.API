'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { BriefingEditor } from '@/components/briefing/briefing-editor'
import { ChecklistComponentEditor } from '@/components/shared/checklist-component-editor'
import { SortableSectionList, type Section } from '@/components/sections/sortable-section-list'
import { SortableChecklistComponents } from '@/components/checklist/sortable-checklist-components'
import { CollapsibleSection } from '@/components/ui/collapsible-section'
import { Button } from '@/components/ui/button'
import { ComponentPreview } from '@/components/preview/component-preview'
import { CreateComponentRequest, CreateChecklistRequest, CreateSectionRequest } from '@/types'
import { useCreateChecklist } from '@/hooks/use-api'
import { useToast } from '@/components/providers/toast-provider'
import { useApiError } from '@/hooks/use-api-error'
import {
  Save,
  Eye,
  EyeOff,
  Plus,
  FileText,
  CheckSquare,
  Share2,
  Info,
  List,
  Clock,
  CheckCircle,
  Upload,
  PenTool,
  Trash2,
  Edit3
} from 'lucide-react'

export default function NewChecklistPage() {
  const router = useRouter()
  const { showToast } = useToast()
  const { handleApiError } = useApiError()
  const createChecklistMutation = useCreateChecklist()

  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [deadline, setDeadline] = useState('')
  const [sections, setSections] = useState<Section[]>([])
  const [activeSection, setActiveSection] = useState<number | null>(null)
  const [showPreview, setShowPreview] = useState(false)
  const [sectionEditingStates, setSectionEditingStates] = useState<Record<number, boolean>>({})
  const [basicInfoExpanded, setBasicInfoExpanded] = useState(true)
  const [sectionsExpanded, setSectionsExpanded] = useState(true)
  const [showNewComponentEditor, setShowNewComponentEditor] = useState(false)
  const [editingComponentId, setEditingComponentId] = useState<number | null>(null)

  const addSection = (type: 'briefing' | 'checklist') => {
    const newSection: Section = {
      id: Date.now(), // Usar timestamp como ID temporário
      title: type === 'briefing' ? 'Nova Seção de Briefing' : 'Nova Seção de Checklist',
      type,
      order: sections.length + 1,
      components: type === 'checklist' ? [] : undefined,
      contentHtml: type === 'briefing' ? '' : undefined,
      contentJson: type === 'briefing' ? '' : undefined
    }

    setSections(prev => [...prev, newSection])
    setActiveSection(newSection.id)

    // Inicializar estado de edição para nova seção de briefing
    if (type === 'briefing') {
      setSectionEditingStates(prev => ({
        ...prev,
        [newSection.id]: true
      }))
    }
  }

  const updateSection = (sectionId: number, updates: Partial<Section>) => {
    setSections(prev => prev.map(section =>
      section.id === sectionId ? { ...section, ...updates } : section
    ))
  }

  const deleteSection = (sectionId: number) => {
    setSections(prev => prev.filter(section => section.id !== sectionId))
    if (activeSection === sectionId) {
      setActiveSection(null)
    }
  }

  const handleBriefingSave = (sectionId: number, content: { contentJson: string; contentHtml: string }) => {
    console.log('🎯 Salvando briefing para seção:', sectionId, content)
    updateSection(sectionId, content)
    console.log('✅ Seção atualizada')
  }

  const handleSectionEditingChange = (sectionId: number, isEditing: boolean) => {
    setSectionEditingStates(prev => ({
      ...prev,
      [sectionId]: isEditing
    }))
  }

  const handleSectionsReorder = (reorderedSections: Section[]) => {
    setSections(reorderedSections)
  }

  const handleComponentSave = (sectionId: number, component: CreateComponentRequest) => {
    setSections(prev => prev.map(section => {
      if (section.id === sectionId) {
        const components = section.components || []

        if (editingComponentId) {
          // Editando componente existente
          return {
            ...section,
            components: components.map(existingComponent =>
              existingComponent.id === editingComponentId
                ? { ...component, id: editingComponentId }
                : existingComponent
            )
          }
        } else {
          // Criando novo componente
          return {
            ...section,
            components: [...components, { ...component, id: Math.random().toString(36).substr(2, 9) }]
          }
        }
      }
      return section
    }))

    // Esconder editor e resetar estados
    setShowNewComponentEditor(false)
    setEditingComponentId(null)
  }

  const handleEditComponent = (sectionId: number, componentId: number) => {
    setEditingComponentId(componentId)
    setShowNewComponentEditor(true)
  }

  const handleDeleteComponent = (sectionId: number, componentId: number) => {
    setSections(prev => prev.map(section => {
      if (section.id === sectionId) {
        return {
          ...section,
          components: section.components?.filter(component => component.id !== componentId) || []
        }
      }
      return section
    }))
  }

  const handleCancelComponentEdit = () => {
    setShowNewComponentEditor(false)
    setEditingComponentId(null)
  }

  const handleComponentsReorder = (sectionId: number, reorderedComponents: any[]) => {
    setSections(prev => prev.map(section => {
      if (section.id === sectionId) {
        return {
          ...section,
          components: reorderedComponents
        }
      }
      return section
    }))
  }

  const handleSaveChecklist = async () => {
    if (!title.trim()) {
      showToast({
        type: 'error',
        title: 'Erro de validação',
        description: 'O título do checklist é obrigatório'
      })
      return
    }

    // Converter sections do estado local para o formato da API
    const sectionsForAPI: CreateSectionRequest[] = sections.map(section => ({
      title: section.title,
      type: section.type,
      order: section.order,
      contentJson: section.contentJson,
      contentHtml: section.contentHtml,
      components: section.type === 'checklist' ? section.components?.map(component => {
        // Remover propriedades que não existem na API e garantir que está no formato correto
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const { id, ...componentWithoutId } = component

        // Garantir que o componente está no formato correto
        const cleanComponent = {
          title: componentWithoutId.title,
          description: componentWithoutId.description,
          isRequired: componentWithoutId.isRequired,
          order: componentWithoutId.order,
          type: componentWithoutId.type,
          // Propriedades específicas por tipo
          ...(componentWithoutId.type === 'text' && {
            placeholder: componentWithoutId.placeholder,
            maxLength: componentWithoutId.maxLength
          }),
          ...(componentWithoutId.type === 'upload' && {
            placeholder: componentWithoutId.placeholder,
            allowedFileTypeIds: componentWithoutId.allowedFileTypeIds,
            fileTypeSizeConfigs: componentWithoutId.fileTypeSizeConfigs
          }),
          ...(componentWithoutId.type === 'confirmation' && {
            confirmationText: componentWithoutId.confirmationText
          })
        }

        return cleanComponent as CreateComponentRequest
      }) : undefined
    }))

    const checklistData: CreateChecklistRequest = {
      title: title.trim(),
      description: description.trim() || undefined,
      deadline: deadline || undefined,
      sections: sectionsForAPI
    }

    try {
      const result = await createChecklistMutation.mutateAsync(checklistData)
      
      showToast({
        type: 'success',
        title: 'Checklist salvo!',
        description: 'O checklist foi criado com sucesso'
      })

      // Redirecionar para a página de edição do checklist criado
      router.push(`/checklists/${result.id}`)
    } catch (error) {
      // Usar o sistema de tratamento de erros
      handleApiError(error, 'Erro ao salvar checklist')
    }
  }

  const handlePublishChecklist = () => {
    // TODO: Implementar publicação
    console.log('Publicando checklist...')
  }

  const activeSecData = sections.find(s => s.id === activeSection)

  return (
    <DashboardLayout showSearchBar={false}>
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
                  <h1 className="text-2xl font-bold text-gray-900">Novo Checklist</h1>
                  <div className="flex items-center space-x-2 mt-1">
                    <span className="text-sm text-gray-600">
                      {title || 'Sem título'}
                    </span>
                    <span className="text-gray-300">•</span>
                    <span className="text-sm text-gray-600">
                      {sections.length} seç{sections.length !== 1 ? 'ões' : 'ão'}
                    </span>
                    {sections.length > 0 && (
                      <>
                        <span className="text-gray-300">•</span>
                        <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
                          Rascunho
                        </span>
                      </>
                    )}
                  </div>
                </div>
              </div>

              {/* Action Buttons as Icons */}
              <div className="flex items-center space-x-2">
            <Button
              onClick={() => setShowPreview(!showPreview)}
              variant="ghost"
              size="sm"
              className={`${showPreview
                ? 'text-blue-600 hover:text-blue-700 bg-blue-50 hover:bg-blue-100'
                : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'
              }`}
              title={showPreview ? 'Ocultar Preview' : 'Visualizar Preview'}
            >
              {showPreview ? (
                <Eye className="h-4 w-4" />
              ) : (
                <EyeOff className="h-4 w-4" />
              )}
            </Button>

            <div className="h-4 w-px bg-gray-300"></div>

            <Button
              onClick={handleSaveChecklist}
              disabled={!title.trim() || createChecklistMutation.isPending}
              variant="ghost"
              size="sm"
              className="text-gray-600 hover:text-gray-900 hover:bg-gray-100 disabled:opacity-50"
              title="Salvar Rascunho"
            >
              <Save className={`h-4 w-4 ${createChecklistMutation.isPending ? 'animate-spin' : ''}`} />
            </Button>

            <Button
              onClick={handlePublishChecklist}
              disabled={!title.trim() || sections.length === 0}
              size="sm"
              className="bg-blue-600 hover:bg-blue-700 text-white shadow-sm disabled:opacity-50"
              title="Publicar Checklist"
            >
              <Share2 className="h-4 w-4" />
            </Button>
              </div>
            </div>
          </div>

          {/* Main Content */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
            <div className="flex h-[calc(100vh-200px)]">
              {/* Left Panel - Structure */}
              <div className="w-80 bg-gray-50 border-r border-gray-200 flex flex-col h-full">
                {/* Scrollable Content Container */}
                <div className="flex-1 overflow-y-auto">
                  {/* Basic Info - Collapsible */}
                  <CollapsibleSection
                    title="Informações Básicas"
                    icon={<Info className="h-4 w-4" />}
                    defaultExpanded={true}
                    className="border-b-0"
                    onToggle={setBasicInfoExpanded}
                  >
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Título *
                  </label>
                  <input
                    type="text"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder="Ex: Onboarding - Redesign Website"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Descrição
                  </label>
                  <textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Breve descrição do projeto..."
                    rows={3}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Prazo
                  </label>
                  <input
                    type="date"
                    value={deadline}
                    onChange={(e) => setDeadline(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                    </div>
                  </div>
                  </CollapsibleSection>

                  {/* Sections - Collapsible */}
                  <CollapsibleSection
                    title={`Seções (${sections.length})`}
                    icon={<List className="h-4 w-4" />}
                    defaultExpanded={true}
                    className="flex-1 flex flex-col"
                    contentClassName="flex-1 p-0"
                    onToggle={setSectionsExpanded}
                  >
                    <div className="mb-4">
                      <div className="flex space-x-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => addSection('briefing')}
                          className="flex-1 text-xs"
                        >
                          <FileText className="h-3 w-3 mr-1" />
                          Briefing
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => addSection('checklist')}
                          className="flex-1 text-xs"
                        >
                          <CheckSquare className="h-3 w-3 mr-1" />
                          Checklist
                        </Button>
                      </div>
                    </div>

                    {/* Sections List */}
                    <div className="flex-1">
                      <SortableSectionList
                        sections={sections}
                        activeSection={activeSection}
                        onSectionClick={setActiveSection}
                        onSectionDelete={deleteSection}
                        onSectionsReorder={handleSectionsReorder}
                      />
                    </div>
                  </CollapsibleSection>
                </div>


        </div>

        {/* Right Panel - Editor/Preview */}
        <div className="flex-1 bg-gray-50">
          {showPreview ? (
            /* Preview Mode - Cliente View */
            <div className="h-full overflow-auto bg-gray-50">
              {/* Header como na página do cliente */}
              <header className="bg-white shadow-sm border-b">
                <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
                  <div className="text-center">
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">
                      {title || 'Título do Checklist'}
                    </h1>
                    {description && (
                      <p className="text-lg text-gray-600 mb-4">
                        {description}
                      </p>
                    )}

                    {/* Progress Bar */}
                    <div className="max-w-md mx-auto">
                      <div className="flex justify-between text-sm text-gray-600 mb-2">
                        <span>Progresso</span>
                        <span>0/{sections.reduce((acc, s) => acc + (s.components?.length || 0), 0)} componentes</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-3">
                        <div
                          className="bg-blue-600 h-3 rounded-full transition-all duration-500"
                          style={{ width: '0%' }}
                        />
                      </div>
                      <p className="text-sm text-gray-500 mt-2">
                        0% concluído
                      </p>
                    </div>

                    {deadline && (
                      <div className="flex items-center justify-center mt-4 text-sm text-gray-600">
                        <Clock className="h-4 w-4 mr-2" />
                        Prazo: {deadline}
                      </div>
                    )}
                  </div>
                </div>
              </header>

              {/* Content como na página do cliente */}
              <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {sections.length === 0 ? (
                  <div className="text-center py-12 text-gray-500">
                    <FileText className="h-16 w-16 mx-auto mb-4 text-gray-300" />
                    <p>Nenhuma seção criada ainda</p>
                    <p className="text-sm mt-2">Adicione seções para ver como ficará para o cliente</p>
                  </div>
                ) : (
                  <div className="space-y-8">
                    {sections.map((section) => (
                      <div key={section.id} className="bg-white rounded-lg shadow-sm border border-gray-200">
                        <div className="px-6 py-4 border-b border-gray-200">
                          <h2 className="text-xl font-semibold text-gray-900">
                            {section.title}
                          </h2>
                        </div>

                        <div className="p-6">
                          {section.type === 'briefing' && section.contentHtml && (
                            <div
                              className="prose max-w-none"
                              dangerouslySetInnerHTML={{ __html: section.contentHtml }}
                            />
                          )}

                          {section.type === 'checklist' && (
                            <div className="space-y-6">
                              {section.components?.length ? (
                                section.components.map((component, index) => (
                                  <ComponentPreview key={component.id || index} component={component} />
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
                )}
              </main>
            </div>
          ) : activeSecData ? (
            /* Editor Mode */
            <div className="h-full flex flex-col">
              {/* Section Header */}
              <div className="bg-white border-b border-gray-200 p-6">
                <div className="flex items-center justify-between">
                  <div className="flex-1">
                    <div className="group flex items-center space-x-2">
                      <input
                        type="text"
                        value={activeSecData.title}
                        onChange={(e) => updateSection(activeSecData.id, { title: e.target.value })}
                        className="text-xl font-bold text-gray-900 bg-transparent border-none outline-none focus:ring-0 p-0 flex-1 hover:bg-gray-50 focus:bg-gray-50 rounded px-2 py-1 -mx-2 -my-1 transition-colors"
                        placeholder="Digite o título da seção..."
                      />
                      <Edit3 className="h-4 w-4 text-gray-400 opacity-0 group-hover:opacity-100 transition-opacity" />
                    </div>
                    <p className="text-sm text-gray-500 mt-1 ml-2">
                      {activeSecData.type === 'briefing' ? 'Seção de Briefing' : 'Seção de Checklist'}
                    </p>
                  </div>
                </div>
              </div>

              {/* Section Content */}
              <div className="flex-1 p-6 overflow-auto">
                {activeSecData.type === 'briefing' ? (
                  <BriefingEditor
                    key={activeSecData.id}
                    initialContent={activeSecData.contentHtml}
                    onSave={(content) => handleBriefingSave(activeSecData.id, content)}
                    sectionId={activeSecData.id}
                    placeholder="Escreva o briefing desta seção..."
                    isEditing={sectionEditingStates[activeSecData.id] ?? true}
                    onEditingChange={(isEditing) => handleSectionEditingChange(activeSecData.id, isEditing)}
                  />
                ) : (
                  <div className="space-y-6">
                    {/* Existing Components - Sortable */}
                    {activeSecData.components && activeSecData.components.length > 0 && (
                      <SortableChecklistComponents
                        components={activeSecData.components}
                        onComponentsReorder={(reorderedComponents) => handleComponentsReorder(activeSecData.id, reorderedComponents)}
                        onEditComponent={(componentId) => handleEditComponent(activeSecData.id, componentId)}
                        onDeleteComponent={(componentId) => handleDeleteComponent(activeSecData.id, componentId)}
                      />
                    )}

                    {/* New Component Editor - Conditional */}
                    {showNewComponentEditor && (
                      <div className="bg-white rounded-lg border-2 border-dashed border-gray-300 p-6">
                        <ChecklistComponentEditor
                          component={editingComponentId ? activeSecData.components?.find(component => component.id === editingComponentId) : undefined}
                          sectionId={activeSecData.id}
                          order={(activeSecData.components?.length || 0) + 1}
                          onSave={(component) => handleComponentSave(activeSecData.id, component)}
                          onCancel={handleCancelComponentEdit}
                        />
                      </div>
                    )}

                    {/* Add New Component Button */}
                    {!showNewComponentEditor && (
                      <div className="text-center">
                        <Button
                          onClick={() => setShowNewComponentEditor(true)}
                          variant="outline"
                          className="border-dashed border-2 border-gray-300 hover:border-gray-400 text-gray-600 hover:text-gray-700"
                        >
                          <Plus className="h-4 w-4 mr-2" />
                          Novo Componente (Item)
                        </Button>
                      </div>
                    )}
                  </div>
                )}
              </div>
            </div>
          ) : (
            <div className="h-full flex items-center justify-center text-gray-500">
              <div className="text-center">
                <FileText className="h-16 w-16 mx-auto mb-4 text-gray-300" />
                <h3 className="text-lg font-medium text-gray-900 mb-2">
                  Selecione uma seção para editar
                </h3>
                <p className="text-sm">
                  Escolha uma seção na barra lateral ou crie uma nova
                </p>
              </div>
            </div>
          )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </DashboardLayout>
  )
}
