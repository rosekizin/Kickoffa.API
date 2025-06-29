'use client'

import { useState } from 'react'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { BriefingEditor } from '@/components/briefing/briefing-editor'
import { ChecklistItemEditor } from '@/components/shared/checklist-item-editor'
import { SortableSectionList, type Section } from '@/components/sections/sortable-section-list'
import { SortableChecklistItems } from '@/components/checklist/sortable-checklist-items'
import { CollapsibleSection } from '@/components/ui/collapsible-section'
import { Button } from '@/components/ui/button'
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
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [deadline, setDeadline] = useState('')
  const [sections, setSections] = useState<Section[]>([])
  const [activeSection, setActiveSection] = useState<string | null>(null)
  const [showPreview, setShowPreview] = useState(false)
  const [sectionEditingStates, setSectionEditingStates] = useState<Record<string, boolean>>({})
  const [basicInfoExpanded, setBasicInfoExpanded] = useState(true)
  const [sectionsExpanded, setSectionsExpanded] = useState(true)
  const [showNewItemEditor, setShowNewItemEditor] = useState(false)
  const [editingItemId, setEditingItemId] = useState<string | null>(null)


  const addSection = (type: 'briefing' | 'checklist') => {
    const newSection: Section = {
      id: Math.random().toString(36).substr(2, 9),
      title: type === 'briefing' ? 'Nova Seção de Briefing' : 'Nova Seção de Checklist',
      type,
      order: sections.length + 1,
      items: type === 'checklist' ? [] : undefined,
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

  const updateSection = (sectionId: string, updates: Partial<Section>) => {
    setSections(prev => prev.map(section => 
      section.id === sectionId ? { ...section, ...updates } : section
    ))
  }

  const deleteSection = (sectionId: string) => {
    setSections(prev => prev.filter(section => section.id !== sectionId))
    if (activeSection === sectionId) {
      setActiveSection(null)
    }
  }

  const handleBriefingSave = (sectionId: string, content: { contentJson: string; contentHtml: string }) => {
    console.log('🎯 Salvando briefing para seção:', sectionId, content)
    updateSection(sectionId, content)
    console.log('✅ Seção atualizada')
  }

  const handleSectionEditingChange = (sectionId: string, isEditing: boolean) => {
    setSectionEditingStates(prev => ({
      ...prev,
      [sectionId]: isEditing
    }))
  }

  const handleSectionsReorder = (reorderedSections: Section[]) => {
    setSections(reorderedSections)
  }

  const handleItemSave = (sectionId: string, item: any) => {
    setSections(prev => prev.map(section => {
      if (section.id === sectionId) {
        const items = section.items || []

        if (editingItemId) {
          // Editando item existente
          return {
            ...section,
            items: items.map(existingItem =>
              existingItem.id === editingItemId
                ? { ...item, id: editingItemId }
                : existingItem
            )
          }
        } else {
          // Criando novo item
          return {
            ...section,
            items: [...items, { ...item, id: Math.random().toString(36).substr(2, 9) }]
          }
        }
      }
      return section
    }))

    // Esconder editor e resetar estados
    setShowNewItemEditor(false)
    setEditingItemId(null)
  }

  const handleEditItem = (sectionId: string, itemId: string) => {
    setEditingItemId(itemId)
    setShowNewItemEditor(true)
  }

  const handleDeleteItem = (sectionId: string, itemId: string) => {
    setSections(prev => prev.map(section => {
      if (section.id === sectionId) {
        return {
          ...section,
          items: section.items?.filter(item => item.id !== itemId) || []
        }
      }
      return section
    }))
  }

  const handleCancelItemEdit = () => {
    setShowNewItemEditor(false)
    setEditingItemId(null)
  }

  const handleItemsReorder = (sectionId: string, reorderedItems: any[]) => {
    setSections(prev => prev.map(section => {
      if (section.id === sectionId) {
        return {
          ...section,
          items: reorderedItems
        }
      }
      return section
    }))
  }

  const handleSaveChecklist = () => {
    const checklistData = {
      title,
      description,
      deadline,
      sections
    }
    
    console.log('Salvando checklist:', checklistData)
    // TODO: Implementar salvamento real
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
              disabled={!title.trim()}
              variant="ghost"
              size="sm"
              className="text-gray-600 hover:text-gray-900 hover:bg-gray-100 disabled:opacity-50"
              title="Salvar Rascunho"
            >
              <Save className="h-4 w-4" />
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
                        <span>0/{sections.reduce((acc, s) => acc + (s.items?.length || 0), 0)} itens</span>
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
                              {section.items?.length ? (
                                section.items.map((item, index) => (
                                  <div key={item.id || index} className="border border-gray-200 rounded-lg p-4">
                                    <div className="flex items-start space-x-3">
                                      <div className="flex-shrink-0 mt-1">
                                        <Upload className="h-5 w-5 text-blue-600" />
                                      </div>

                                      <div className="flex-1 space-y-3">
                                        <div>
                                          <h3 className="font-medium text-gray-900 flex items-center">
                                            {item.title}
                                            <span className="ml-2 text-red-500 text-sm">*</span>
                                          </h3>
                                          <p className="text-sm text-gray-600 mt-1">
                                            Item do checklist para preenchimento pelo cliente
                                          </p>
                                        </div>

                                        <div className="bg-gray-50 border-2 border-dashed border-gray-300 rounded-lg p-4 text-center">
                                          <Upload className="h-8 w-8 text-gray-400 mx-auto mb-2" />
                                          <p className="text-sm text-gray-600">
                                            Campo interativo aparecerá aqui para o cliente
                                          </p>
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                ))
                              ) : (
                                <p className="text-gray-500 italic">Nenhum item adicionado ainda</p>
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
                  <div>
                    <input
                      type="text"
                      value={activeSecData.title}
                      onChange={(e) => updateSection(activeSecData.id, { title: e.target.value })}
                      className="text-xl font-bold text-gray-900 bg-transparent border-none outline-none focus:ring-0 p-0"
                    />
                    <p className="text-sm text-gray-500 mt-1">
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
                    {/* Existing Items - Sortable */}
                    {activeSecData.items && activeSecData.items.length > 0 && (
                      <SortableChecklistItems
                        items={activeSecData.items}
                        onItemsReorder={(reorderedItems) => handleItemsReorder(activeSecData.id, reorderedItems)}
                        onEditItem={(itemId) => handleEditItem(activeSecData.id, itemId)}
                        onDeleteItem={(itemId) => handleDeleteItem(activeSecData.id, itemId)}
                      />
                    )}

                    {/* New Item Editor - Conditional */}
                    {showNewItemEditor && (
                      <div className="bg-white rounded-lg border-2 border-dashed border-gray-300 p-6">
                        <ChecklistItemEditor
                          item={editingItemId ? activeSecData.items?.find(item => item.id === editingItemId) : undefined}
                          sectionId={activeSecData.id}
                          order={(activeSecData.items?.length || 0) + 1}
                          onSave={(item) => handleItemSave(activeSecData.id, item)}
                          onCancel={handleCancelItemEdit}
                        />
                      </div>
                    )}

                    {/* Add New Item Button */}
                    {!showNewItemEditor && (
                      <div className="text-center">
                        <Button
                          onClick={() => setShowNewItemEditor(true)}
                          variant="outline"
                          className="border-dashed border-2 border-gray-300 hover:border-gray-400 text-gray-600 hover:text-gray-700"
                        >
                          <Plus className="h-4 w-4 mr-2" />
                          Novo Item
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
