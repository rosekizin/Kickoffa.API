'use client'

import { useState } from 'react'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { BriefingEditor } from '@/components/briefing/briefing-editor'
import { ChecklistItemEditor } from '@/components/shared/checklist-item-editor'
import { SortableSectionList, type Section } from '@/components/sections/sortable-section-list'
import { Button } from '@/components/ui/button'
import {
  Save,
  Eye,
  Plus,
  FileText,
  CheckSquare,
  Settings,
  Share2
} from 'lucide-react'

export default function NewChecklistPage() {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [deadline, setDeadline] = useState('')
  const [sections, setSections] = useState<Section[]>([])
  const [activeSection, setActiveSection] = useState<string | null>(null)
  const [showPreview, setShowPreview] = useState(false)
  const [sectionEditingStates, setSectionEditingStates] = useState<Record<string, boolean>>({})

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
        return {
          ...section,
          items: [...items, { ...item, id: Math.random().toString(36).substr(2, 9) }]
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
    <DashboardLayout>
      <div className="flex flex-col h-screen">
        {/* Top Header with Action Buttons */}
        <div className="bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center shadow-sm">
          <div>
            <h1 className="text-xl font-bold text-gray-900">Novo Checklist</h1>
            <div className="flex items-center space-x-2 mt-1">
              <span className="text-sm text-gray-500">
                {title || 'Sem título'}
              </span>
              <span className="text-gray-300">•</span>
              <span className="text-sm text-gray-500">
                {sections.length} seção{sections.length !== 1 ? 'ões' : ''}
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

          {/* Action Buttons as Icons */}
          <div className="flex items-center space-x-2">
            <Button
              onClick={() => setShowPreview(!showPreview)}
              variant="ghost"
              size="sm"
              className="text-gray-600 hover:text-gray-900 hover:bg-gray-100"
              title={showPreview ? 'Ocultar Preview' : 'Visualizar Preview'}
            >
              <Eye className="h-4 w-4" />
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

        {/* Main Content */}
        <div className="flex flex-1">
          {/* Left Panel - Structure */}
          <div className="w-80 bg-white border-r border-gray-200 flex flex-col h-full">
            {/* Basic Info */}
            <div className="p-6 border-b border-gray-200">
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
          </div>

          {/* Sections */}
          <div className="flex-1 p-6">
            <div className="flex justify-between items-center mb-4">
              <h3 className="font-medium text-gray-900">Seções</h3>
              <div className="flex space-x-1">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => addSection('briefing')}
                  className="text-xs"
                >
                  <FileText className="h-3 w-3 mr-1" />
                  Briefing
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => addSection('checklist')}
                  className="text-xs"
                >
                  <CheckSquare className="h-3 w-3 mr-1" />
                  Checklist
                </Button>
              </div>
            </div>

            {/* Sections List */}
            <div className="flex-1 overflow-y-auto" style={{ maxHeight: 'calc(100vh - 400px)' }}>
              <div className="p-6">
                <SortableSectionList
                  sections={sections}
                  activeSection={activeSection}
                  onSectionClick={setActiveSection}
                  onSectionDelete={deleteSection}
                  onSectionsReorder={handleSectionsReorder}
                />
              </div>
            </div>
          </div>


        </div>

        {/* Right Panel - Editor/Preview */}
        <div className="flex-1 bg-gray-50">
          {showPreview ? (
            /* Preview Mode */
            <div className="h-full flex flex-col">
              <div className="bg-white border-b border-gray-200 p-6">
                <h2 className="text-2xl font-bold text-gray-900">{title || 'Título do Checklist'}</h2>
                {description && <p className="text-gray-600 mt-2">{description}</p>}
                {deadline && <p className="text-sm text-gray-500 mt-2">Prazo: {deadline}</p>}
              </div>
              <div className="flex-1 p-6 overflow-auto">
                {sections.length === 0 ? (
                  <div className="text-center py-12 text-gray-500">
                    <FileText className="h-16 w-16 mx-auto mb-4 text-gray-300" />
                    <p>Nenhuma seção criada ainda</p>
                  </div>
                ) : (
                  <div className="space-y-6">
                    {sections.map((section) => (
                      <div key={section.id} className="bg-white rounded-lg border border-gray-200 p-6">
                        <h3 className="text-lg font-semibold mb-4 flex items-center">
                          {section.type === 'briefing' ? (
                            <FileText className="h-5 w-5 text-blue-600 mr-2" />
                          ) : (
                            <CheckSquare className="h-5 w-5 text-green-600 mr-2" />
                          )}
                          {section.title}
                        </h3>
                        {section.type === 'briefing' ? (
                          <div
                            className="prose max-w-none"
                            dangerouslySetInnerHTML={{ __html: section.contentHtml || '<p>Conteúdo do briefing...</p>' }}
                          />
                        ) : (
                          <div className="space-y-3">
                            {section.items?.length ? (
                              section.items.map((item, index) => (
                                <div key={index} className="flex items-center space-x-3 p-3 bg-gray-50 rounded">
                                  <CheckSquare className="h-4 w-4 text-gray-400" />
                                  <span>{item.title || `Item ${index + 1}`}</span>
                                </div>
                              ))
                            ) : (
                              <p className="text-gray-500 italic">Nenhum item adicionado</p>
                            )}
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </div>
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
                  <Button variant="ghost" size="sm">
                    <Settings className="h-4 w-4" />
                  </Button>
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
                    {/* Existing Items */}
                    {activeSecData.items?.map((item, index) => (
                      <div key={item.id} className="bg-white rounded-lg border border-gray-200 p-4">
                        <div className="flex items-center justify-between">
                          <div>
                            <h4 className="font-medium text-gray-900">{item.title}</h4>
                            <p className="text-sm text-gray-500">{item.type}</p>
                          </div>
                          <Button variant="ghost" size="sm">
                            <Trash2 className="h-4 w-4 text-red-500" />
                          </Button>
                        </div>
                      </div>
                    ))}
                    
                    {/* Add New Item */}
                    <div className="bg-white rounded-lg border-2 border-dashed border-gray-300 p-6">
                      <ChecklistItemEditor
                        sectionId={activeSecData.id}
                        order={(activeSecData.items?.length || 0) + 1}
                        onSave={(item) => handleItemSave(activeSecData.id, item)}
                      />
                    </div>
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
    </DashboardLayout>
  )
}
