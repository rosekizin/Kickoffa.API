'use client'

import { useState } from 'react'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { BriefingEditor } from '@/components/briefing/briefing-editor'
import { ChecklistItemEditor } from '@/components/shared/checklist-item-editor'
import { Button } from '@/components/ui/button'
import { 
  Save, 
  Eye, 
  Plus, 
  FileText, 
  CheckSquare,
  GripVertical,
  Trash2,
  Settings,
  Share2
} from 'lucide-react'

interface Section {
  id: string
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentJson?: string
  contentHtml?: string
  items?: any[]
}

export default function NewChecklistPage() {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [deadline, setDeadline] = useState('')
  const [sections, setSections] = useState<Section[]>([])
  const [activeSection, setActiveSection] = useState<string | null>(null)
  const [showPreview, setShowPreview] = useState(false)

  const addSection = (type: 'briefing' | 'checklist') => {
    const newSection: Section = {
      id: Math.random().toString(36).substr(2, 9),
      title: type === 'briefing' ? 'Nova Seção de Briefing' : 'Nova Seção de Checklist',
      type,
      order: sections.length + 1,
      items: type === 'checklist' ? [] : undefined
    }
    
    setSections(prev => [...prev, newSection])
    setActiveSection(newSection.id)
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
    updateSection(sectionId, content)
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
      <div className="flex h-screen">
        {/* Left Panel - Structure */}
        <div className="w-80 bg-white border-r border-gray-200 flex flex-col">
          {/* Header */}
          <div className="p-6 border-b border-gray-200">
            <h1 className="text-xl font-bold text-gray-900 mb-4">Novo Checklist</h1>
            
            {/* Basic Info */}
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

            <div className="space-y-2">
              {sections.map((section) => (
                <div
                  key={section.id}
                  className={`p-3 border rounded-lg cursor-pointer transition-colors ${
                    activeSection === section.id
                      ? 'border-blue-500 bg-blue-50'
                      : 'border-gray-200 hover:border-gray-300'
                  }`}
                  onClick={() => setActiveSection(section.id)}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center space-x-2">
                      <GripVertical className="h-4 w-4 text-gray-400" />
                      {section.type === 'briefing' ? (
                        <FileText className="h-4 w-4 text-blue-600" />
                      ) : (
                        <CheckSquare className="h-4 w-4 text-green-600" />
                      )}
                      <span className="text-sm font-medium text-gray-900">
                        {section.title}
                      </span>
                    </div>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={(e) => {
                        e.stopPropagation()
                        deleteSection(section.id)
                      }}
                      className="h-6 w-6 p-0 text-gray-400 hover:text-red-500"
                    >
                      <Trash2 className="h-3 w-3" />
                    </Button>
                  </div>
                  
                  {section.type === 'checklist' && section.items && (
                    <div className="mt-2 text-xs text-gray-500">
                      {section.items.length} item{section.items.length !== 1 ? 's' : ''}
                    </div>
                  )}
                </div>
              ))}
              
              {sections.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                  <FileText className="h-8 w-8 mx-auto mb-2 text-gray-300" />
                  <p className="text-sm">Nenhuma seção criada</p>
                  <p className="text-xs">Clique nos botões acima para adicionar</p>
                </div>
              )}
            </div>
          </div>

          {/* Actions */}
          <div className="p-6 border-t border-gray-200 space-y-2">
            <Button
              onClick={handleSaveChecklist}
              disabled={!title.trim()}
              className="w-full bg-blue-600 hover:bg-blue-700"
            >
              <Save className="h-4 w-4 mr-2" />
              Salvar Rascunho
            </Button>
            
            <Button
              onClick={handlePublishChecklist}
              disabled={!title.trim() || sections.length === 0}
              variant="outline"
              className="w-full"
            >
              <Share2 className="h-4 w-4 mr-2" />
              Publicar
            </Button>
            
            <Button
              onClick={() => setShowPreview(!showPreview)}
              variant="ghost"
              className="w-full"
            >
              <Eye className="h-4 w-4 mr-2" />
              {showPreview ? 'Ocultar' : 'Visualizar'} Preview
            </Button>
          </div>
        </div>

        {/* Right Panel - Editor */}
        <div className="flex-1 bg-gray-50">
          {activeSecData ? (
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
                    initialContent={activeSecData.contentHtml}
                    onSave={(content) => handleBriefingSave(activeSecData.id, content)}
                    sectionId={activeSecData.id}
                    placeholder="Escreva o briefing desta seção..."
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
    </DashboardLayout>
  )
}
