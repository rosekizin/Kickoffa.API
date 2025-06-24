'use client'

import { useState, useEffect } from 'react'
import { useParams } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { FileUploader } from '@/components/shared/file-uploader'
import { SignaturePad } from '@/components/shared/signature-pad'
import { 
  CheckCircle, 
  Clock, 
  FileText, 
  Upload, 
  PenTool, 
  AlertCircle,
  Download,
  Share2
} from 'lucide-react'

// Mock data - será substituído por dados reais da API
const mockChecklist = {
  id: '1',
  title: 'Onboarding - Redesign Website',
  description: 'Precisamos coletar algumas informações e arquivos antes de começar o projeto de redesign do seu website.',
  deadline: '2024-01-15',
  sections: [
    {
      id: '1',
      title: 'Briefing do Projeto',
      type: 'briefing' as const,
      order: 1,
      contentHtml: `
        <h2>Objetivo do Projeto</h2>
        <p>Redesign completo do website corporativo com foco em:</p>
        <ul>
          <li>Melhor experiência do usuário</li>
          <li>Design moderno e responsivo</li>
          <li>Otimização para conversões</li>
          <li>Integração com sistemas existentes</li>
        </ul>
        
        <h3>Público-alvo</h3>
        <p>Empresas de médio porte que buscam soluções tecnológicas inovadoras.</p>
        
        <h3>Prazo</h3>
        <p>O projeto deve ser concluído em 6 semanas a partir da aprovação do briefing.</p>
      `
    },
    {
      id: '2',
      title: 'Documentos e Assets',
      type: 'checklist' as const,
      order: 2,
      items: [
        {
          id: '1',
          title: 'Logo da empresa',
          description: 'Envie o logo em alta resolução (PNG ou SVG)',
          type: 'upload' as const,
          isRequired: true,
          order: 1,
          isCompleted: false,
          config: {
            allowedFileTypes: ['.png', '.svg', '.jpg'],
            maxFileSize: 5 * 1024 * 1024,
            maxFiles: 3
          }
        },
        {
          id: '2',
          title: 'Paleta de cores',
          description: 'Descreva as cores da marca ou envie um guia de estilo',
          type: 'text' as const,
          isRequired: true,
          order: 2,
          isCompleted: false,
          config: {
            placeholder: 'Ex: Azul #0066CC, Branco #FFFFFF...',
            multiline: true
          }
        },
        {
          id: '3',
          title: 'Referências visuais',
          description: 'Sites que você gosta ou que servem de inspiração',
          type: 'text' as const,
          isRequired: false,
          order: 3,
          isCompleted: false,
          config: {
            placeholder: 'Cole os links dos sites aqui...',
            multiline: true
          }
        },
        {
          id: '4',
          title: 'Contrato assinado',
          description: 'Upload do contrato devidamente assinado',
          type: 'upload' as const,
          isRequired: true,
          order: 4,
          isCompleted: false,
          config: {
            allowedFileTypes: ['.pdf'],
            maxFileSize: 10 * 1024 * 1024,
            maxFiles: 1
          }
        },
        {
          id: '5',
          title: 'Aprovação final',
          description: 'Confirme que todas as informações estão corretas',
          type: 'signature' as const,
          isRequired: true,
          order: 5,
          isCompleted: false
        }
      ]
    }
  ],
  progress: {
    totalItems: 5,
    completedItems: 0,
    percentage: 0
  }
}

export default function PublicChecklistPage() {
  const params = useParams()
  const token = params.token as string
  
  const [checklist, setChecklist] = useState(mockChecklist)
  const [itemResponses, setItemResponses] = useState<Record<string, any>>({})
  const [isLoading, setIsLoading] = useState(false)

  const handleItemComplete = (itemId: string, value: any) => {
    setItemResponses(prev => ({ ...prev, [itemId]: value }))
    
    // Atualizar status do item
    setChecklist(prev => ({
      ...prev,
      sections: prev.sections.map(section => ({
        ...section,
        items: section.items?.map(item => 
          item.id === itemId 
            ? { ...item, isCompleted: true }
            : item
        )
      }))
    }))

    // Recalcular progresso
    const totalItems = checklist.sections.reduce((acc, section) => 
      acc + (section.items?.length || 0), 0
    )
    const completedItems = Object.keys(itemResponses).length + 1
    
    setChecklist(prev => ({
      ...prev,
      progress: {
        totalItems,
        completedItems,
        percentage: Math.round((completedItems / totalItems) * 100)
      }
    }))
  }

  const handleFilesSelected = (itemId: string, files: File[]) => {
    handleItemComplete(itemId, files)
  }

  const handleTextChange = (itemId: string, text: string) => {
    if (text.trim()) {
      handleItemComplete(itemId, text)
    }
  }

  const handleSignature = (itemId: string, signature: string | null) => {
    if (signature) {
      handleItemComplete(itemId, signature)
    }
  }

  const getItemIcon = (type: string) => {
    switch (type) {
      case 'upload': return <Upload className="h-5 w-5 text-blue-600" />
      case 'text': return <FileText className="h-5 w-5 text-purple-600" />
      case 'signature': return <PenTool className="h-5 w-5 text-orange-600" />
      default: return <CheckCircle className="h-5 w-5 text-green-600" />
    }
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="text-center">
            <h1 className="text-3xl font-bold text-gray-900 mb-2">
              {checklist.title}
            </h1>
            {checklist.description && (
              <p className="text-lg text-gray-600 mb-4">
                {checklist.description}
              </p>
            )}
            
            {/* Progress Bar */}
            <div className="max-w-md mx-auto">
              <div className="flex justify-between text-sm text-gray-600 mb-2">
                <span>Progresso</span>
                <span>{checklist.progress.completedItems}/{checklist.progress.totalItems} itens</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-3">
                <div 
                  className="bg-blue-600 h-3 rounded-full transition-all duration-500"
                  style={{ width: `${checklist.progress.percentage}%` }}
                />
              </div>
              <p className="text-sm text-gray-500 mt-2">
                {checklist.progress.percentage}% concluído
              </p>
            </div>

            {checklist.deadline && (
              <div className="flex items-center justify-center mt-4 text-sm text-gray-600">
                <Clock className="h-4 w-4 mr-2" />
                Prazo: {checklist.deadline}
              </div>
            )}
          </div>
        </div>
      </header>

      {/* Content */}
      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="space-y-8">
          {checklist.sections.map((section) => (
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

                {section.type === 'checklist' && section.items && (
                  <div className="space-y-6">
                    {section.items.map((item) => (
                      <div key={item.id} className="border border-gray-200 rounded-lg p-4">
                        <div className="flex items-start space-x-3">
                          <div className="flex-shrink-0 mt-1">
                            {item.isCompleted ? (
                              <CheckCircle className="h-5 w-5 text-green-500" />
                            ) : (
                              getItemIcon(item.type)
                            )}
                          </div>
                          
                          <div className="flex-1 space-y-3">
                            <div>
                              <h3 className="font-medium text-gray-900 flex items-center">
                                {item.title}
                                {item.isRequired && (
                                  <span className="ml-2 text-red-500 text-sm">*</span>
                                )}
                              </h3>
                              {item.description && (
                                <p className="text-sm text-gray-600 mt-1">
                                  {item.description}
                                </p>
                              )}
                            </div>

                            {!item.isCompleted && (
                              <div>
                                {item.type === 'upload' && (
                                  <FileUploader
                                    onFilesSelected={(files) => handleFilesSelected(item.id, files)}
                                    maxFiles={item.config?.maxFiles || 1}
                                    maxSize={item.config?.maxFileSize || 10 * 1024 * 1024}
                                    acceptedTypes={item.config?.allowedFileTypes || ['*']}
                                    className="max-w-lg"
                                  />
                                )}

                                {item.type === 'text' && (
                                  <div>
                                    {item.config?.multiline ? (
                                      <textarea
                                        placeholder={item.config?.placeholder || 'Digite sua resposta...'}
                                        rows={4}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        onBlur={(e) => handleTextChange(item.id, e.target.value)}
                                      />
                                    ) : (
                                      <input
                                        type="text"
                                        placeholder={item.config?.placeholder || 'Digite sua resposta...'}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        onBlur={(e) => handleTextChange(item.id, e.target.value)}
                                      />
                                    )}
                                  </div>
                                )}

                                {item.type === 'signature' && (
                                  <SignaturePad
                                    onSignatureChange={(signature) => handleSignature(item.id, signature)}
                                    className="max-w-lg"
                                  />
                                )}
                              </div>
                            )}

                            {item.isCompleted && (
                              <div className="flex items-center text-green-600 text-sm">
                                <CheckCircle className="h-4 w-4 mr-2" />
                                Concluído
                              </div>
                            )}
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>

        {/* Actions */}
        {checklist.progress.percentage === 100 && (
          <div className="bg-green-50 border border-green-200 rounded-lg p-6 text-center">
            <CheckCircle className="h-12 w-12 text-green-500 mx-auto mb-4" />
            <h3 className="text-lg font-semibold text-green-900 mb-2">
              Checklist Concluído!
            </h3>
            <p className="text-green-700 mb-4">
              Obrigado por fornecer todas as informações necessárias. 
              Entraremos em contato em breve para dar início ao projeto.
            </p>
            <div className="flex justify-center space-x-3">
              <Button className="bg-green-600 hover:bg-green-700">
                <Download className="h-4 w-4 mr-2" />
                Baixar Comprovante
              </Button>
              <Button variant="outline">
                <Share2 className="h-4 w-4 mr-2" />
                Compartilhar
              </Button>
            </div>
          </div>
        )}
      </main>
    </div>
  )
}
