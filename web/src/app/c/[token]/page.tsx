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
  Share2,
  Edit3,
  X,
  File,
  Image,
  FileType
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
          description: 'Envie o logo em alta resolução (PNG, SVG ou JPG) - até 3 arquivos',
          type: 'upload' as const,
          isRequired: true,
          order: 1,
          isCompleted: false,
          config: {
            allowedFileTypes: ['image/*'],
            maxFileSize: 5 * 1024 * 1024,
            maxFiles: 3
          }
        },
        {
          id: '2',
          title: 'Paleta de cores',
          description: 'Descreva as cores da marca, tons preferidos e estilo visual',
          type: 'text' as const,
          isRequired: true,
          order: 2,
          isCompleted: false,
          config: {
            placeholder: 'Exemplo:\n\nCores principais:\n- Azul corporativo: #0066CC\n- Branco: #FFFFFF\n- Cinza: #666666\n\nEstilo:\n- Moderno e profissional\n- Minimalista\n- Transmite confiança',
            multiline: true
          }
        },
        {
          id: '3',
          title: 'Referências visuais e inspirações',
          description: 'Sites, designs ou estilos que você gosta e servem de inspiração',
          type: 'text' as const,
          isRequired: false,
          order: 3,
          isCompleted: false,
          config: {
            placeholder: 'Compartilhe suas inspirações:\n\nSites que gosto:\n- https://exemplo1.com - Layout limpo e moderno\n- https://exemplo2.com - Boa navegação\n\nEstilos que admiro:\n- Design minimalista\n- Cores suaves\n- Tipografia elegante\n\nO que NÃO gosto:\n- Muitas cores\n- Layout confuso',
            multiline: true
          }
        },
        {
          id: '4',
          title: 'Documentos do projeto',
          description: 'Envie documentos relacionados ao projeto (PDFs, Word, etc.) - até 5 arquivos',
          type: 'upload' as const,
          isRequired: true,
          order: 4,
          isCompleted: false,
          config: {
            allowedFileTypes: ['application/pdf', '.doc', '.docx', '.txt'],
            maxFileSize: 10 * 1024 * 1024,
            maxFiles: 5
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
  const [editingItem, setEditingItem] = useState<string | null>(null)

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
    setEditingItem(null)
  }

  const handleTextChange = (itemId: string, text: string) => {
    if (text.trim()) {
      handleItemComplete(itemId, text)
      setEditingItem(null)
    }
  }

  const handleSignature = (itemId: string, signature: string | null) => {
    if (signature) {
      handleItemComplete(itemId, signature)
      setEditingItem(null)
    }
  }

  const handleEditItem = (itemId: string) => {
    setEditingItem(itemId)

    // Marcar item como não concluído para permitir edição
    setChecklist(prev => ({
      ...prev,
      sections: prev.sections.map(section => ({
        ...section,
        items: section.items?.map(item =>
          item.id === itemId
            ? { ...item, isCompleted: false }
            : item
        )
      }))
    }))
  }

  const handleCancelEdit = (itemId: string) => {
    setEditingItem(null)

    // Se tinha resposta, marcar como concluído novamente
    if (itemResponses[itemId]) {
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
    }
  }

  const getFileIcon = (fileName: string) => {
    const extension = fileName.split('.').pop()?.toLowerCase()
    switch (extension) {
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
      case 'webp':
        return <Image className="h-4 w-4 text-blue-600" />
      case 'pdf':
        return <FileType className="h-4 w-4 text-red-600" />
      default:
        return <File className="h-4 w-4 text-gray-600" />
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
                            <div className="flex justify-between items-start">
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

                              {/* Botões de ação */}
                              {item.isCompleted && editingItem !== item.id && (
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => handleEditItem(item.id)}
                                  className="text-blue-600 hover:text-blue-700"
                                >
                                  <Edit3 className="h-4 w-4 mr-1" />
                                  Editar
                                </Button>
                              )}

                              {editingItem === item.id && (
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => handleCancelEdit(item.id)}
                                  className="text-gray-600 hover:text-gray-700"
                                >
                                  <X className="h-4 w-4 mr-1" />
                                  Cancelar
                                </Button>
                              )}
                            </div>

                            {/* Mostrar resposta quando concluído e não editando */}
                            {item.isCompleted && editingItem !== item.id && itemResponses[item.id] && (
                              <div className="bg-green-50 border border-green-200 rounded-lg p-3">
                                <div className="flex items-center mb-2">
                                  <CheckCircle className="h-4 w-4 text-green-600 mr-2" />
                                  <span className="text-sm font-medium text-green-800">Resposta enviada</span>
                                </div>

                                {/* Mostrar resposta baseada no tipo */}
                                {item.type === 'text' && (
                                  <div className="text-sm text-gray-700 bg-white p-3 rounded border">
                                    <pre className="whitespace-pre-wrap font-sans text-sm leading-relaxed">
                                      {itemResponses[item.id]}
                                    </pre>
                                  </div>
                                )}

                                {item.type === 'upload' && Array.isArray(itemResponses[item.id]) && (
                                  <div className="space-y-2">
                                    {itemResponses[item.id].map((file: File, index: number) => (
                                      <div key={index} className="flex items-center space-x-2 bg-white p-2 rounded border">
                                        {getFileIcon(file.name)}
                                        <span className="text-sm text-gray-700 flex-1">{file.name}</span>
                                        <span className="text-xs text-gray-500">{formatFileSize(file.size)}</span>
                                      </div>
                                    ))}
                                  </div>
                                )}

                                {item.type === 'signature' && (
                                  <div className="bg-white p-2 rounded border">
                                    <img
                                      src={itemResponses[item.id]}
                                      alt="Assinatura"
                                      className="max-w-xs h-20 object-contain border rounded"
                                    />
                                  </div>
                                )}
                              </div>
                            )}

                            {/* Campos de edição */}
                            {(!item.isCompleted || editingItem === item.id) && (
                              <div>
                                {item.type === 'upload' && (
                                  <FileUploader
                                    onFilesSelected={(files) => handleFilesSelected(item.id, files)}
                                    maxFiles={item.config?.maxFiles || 5}
                                    maxSize={item.config?.maxFileSize || 10 * 1024 * 1024}
                                    acceptedTypes={item.config?.allowedFileTypes || ['image/*', 'application/pdf', '.doc', '.docx', '.txt']}
                                    multiple={true}
                                    className="max-w-lg"
                                  />
                                )}

                                {item.type === 'text' && (
                                  <div>
                                    <textarea
                                      placeholder={item.config?.placeholder || 'Digite sua resposta...\n\nVocê pode usar quebras de linha e formatação básica.'}
                                      defaultValue={itemResponses[item.id] || ''}
                                      rows={item.config?.multiline ? 6 : 4}
                                      className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-vertical"
                                      onBlur={(e) => handleTextChange(item.id, e.target.value)}
                                      style={{ whiteSpace: 'pre-wrap' }}
                                    />
                                    <p className="text-xs text-gray-500 mt-1">
                                      💡 Dica: Use Enter para quebrar linhas e organize seu texto como preferir
                                    </p>
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
