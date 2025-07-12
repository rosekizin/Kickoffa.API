'use client'

import { useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import {
  CheckSquare,
  Upload,
  Type,
  PenTool,
  Settings,
  Eye,
  Trash2,
  GripVertical,
  Shield,
  Edit3,
  X
} from 'lucide-react'
import { FileTypeSelector } from '@/components/upload/file-type-selector'
import { FileTypeSizeConfigComponent, FileTypeSizeConfig } from '@/components/upload/file-type-size-config'
import { FileType } from '@/types'
import { Component, CreateComponentRequest } from '@/types'

interface ChecklistComponentEditorProps {
  component?: Component
  onSave: (component: CreateComponentRequest) => void
  onDelete?: (componentId: number) => void
  onPreview?: () => void
  onCancel?: () => void
  onEdit?: () => void
  sectionId: number
  order: number
  isViewMode?: boolean
}

type ComponentType = 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'

export const ChecklistComponentEditor = ({
  component,
  onSave,
  onDelete,
  onPreview,
  onCancel,
  onEdit,
  sectionId,
  order,
  isViewMode = false
}: ChecklistComponentEditorProps) => {
  const [title, setTitle] = useState(component?.title || '')
  const [description, setDescription] = useState(component?.description || '')
  const [type, setType] = useState<ComponentType>(component?.type || 'checkbox')
  const [isRequired, setIsRequired] = useState(component?.isRequired || false)
  const [maxSizeMB, setMaxSizeMB] = useState(component?.maxSizeMB || 10)
  const [placeholder, setPlaceholder] = useState(component?.placeholder || '')
  const [maxLength, setMaxLength] = useState(component?.maxLength || 500)
  const [confirmationText, setConfirmationText] = useState(component?.confirmationText || '')
  const [selectedFileTypes, setSelectedFileTypes] = useState<FileType[]>([])
  const [fileTypeSizeConfigs, setFileTypeSizeConfigs] = useState<FileTypeSizeConfig[]>([])
  const [showAdvanced, setShowAdvanced] = useState(false)

  // Inicializar dados do componente quando ele for passado para edição
  useEffect(() => {
    if (component) {
      // Inicializar tipos de arquivo permitidos se for um componente de upload
      if (component.type === 'upload' && component.allowedFileTypes) {
        setSelectedFileTypes(component.allowedFileTypes)

        // Inicializar configurações de tamanho se existirem
        const sizeConfigs: FileTypeSizeConfig[] = component.allowedFileTypes.map(fileType => ({
          fileTypeId: fileType.id,
          maxSizeMB: fileType.recommendedMaxSizeMB || component.maxSizeMB || 10
        }))
        setFileTypeSizeConfigs(sizeConfigs)
      }

      // Mostrar configurações avançadas se for um tipo que as possui e tiver dados específicos
      if (component.type === 'upload' && (component.allowedFileTypes?.length || component.placeholder)) {
        setShowAdvanced(true)
      } else if (component.type === 'text' && (component.placeholder || component.maxLength)) {
        setShowAdvanced(true)
      } else if (component.type === 'confirmation' && component.confirmationText) {
        setShowAdvanced(true)
      }
    }
  }, [
    component?.id,
    component?.type,
    component?.allowedFileTypes,
    component?.placeholder,
    component?.maxLength,
    component?.maxSizeMB,
    component?.confirmationText
  ])

  const componentTypes = [
    {
      type: 'checkbox' as const,
      label: 'Checkbox',
      description: 'Componente simples para marcar como concluído',
      icon: CheckSquare,
      color: 'text-green-600'
    },
    {
      type: 'upload' as const,
      label: 'Upload de Arquivo',
      description: 'Permite ao cliente enviar arquivos',
      icon: Upload,
      color: 'text-blue-600'
    },
    {
      type: 'text' as const,
      label: 'Campo de Texto',
      description: 'Campo para o cliente inserir texto',
      icon: Type,
      color: 'text-purple-600'
    },
    {
      type: 'signature' as const,
      label: 'Assinatura Digital',
      description: 'Captura assinatura do cliente',
      icon: PenTool,
      color: 'text-orange-600'
    },
    {
      type: 'confirmation' as const,
      label: 'Confirmação',
      description: 'Componente de confirmação com texto personalizado',
      icon: Shield,
      color: 'text-indigo-600'
    }
  ]

  const handleSave = () => {
    if (!title.trim()) return

    const baseData = {
      title: title.trim(),
      description: description.trim() || undefined,
      isRequired,
      order
    }

    let componentData: CreateComponentRequest

    switch (type) {
      case 'checkbox':
        componentData = {
          ...baseData,
          type: 'checkbox'
        }
        break

      case 'text':
        componentData = {
          ...baseData,
          type: 'text',
          placeholder: placeholder || undefined,
          maxLength: maxLength || undefined
        }
        break

      case 'upload':
        // Calcular tamanho máximo baseado nas configurações específicas ou usar o padrão
        let calculatedMaxSize = maxSizeMB
        if (fileTypeSizeConfigs.length > 0) {
          calculatedMaxSize = Math.max(...fileTypeSizeConfigs.map(config => config.maxSizeMB))
        }

        componentData = {
          ...baseData,
          type: 'upload',
          placeholder: placeholder || undefined,
          maxSizeMB: calculatedMaxSize || undefined,
          allowedFileTypes: selectedFileTypes,
          allowedFileTypeIds: selectedFileTypes.map(ft => ft.id)
        } as CreateComponentRequest
        break

      case 'signature':
        componentData = {
          ...baseData,
          type: 'signature'
        }
        break

      case 'confirmation':
        componentData = {
          ...baseData,
          type: 'confirmation',
          confirmationText: confirmationText || undefined
        }
        break

      default:
        throw new Error(`Tipo de componente não suportado: ${type}`)
    }

    onSave(componentData)
  }

  const renderTypeSpecificConfig = () => {
    switch (type) {
      case 'upload':
        return (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Tipos de arquivo permitidos
              </label>
              <FileTypeSelector
                selectedFileTypes={selectedFileTypes}
                onSelectionChange={setSelectedFileTypes}
                placeholder="Selecione os tipos de arquivo permitidos..."
                maxSelections={10}
              />
              <p className="text-xs text-gray-500 mt-1">
                Selecione os tipos de arquivo que o cliente poderá enviar
              </p>
            </div>

            {/* Configuração de tamanhos específicos por tipo */}
            {selectedFileTypes.length > 0 && (
              <div>
                <FileTypeSizeConfigComponent
                  selectedFileTypes={selectedFileTypes}
                  sizeConfigs={fileTypeSizeConfigs}
                  onSizeConfigsChange={setFileTypeSizeConfigs}
                  globalMaxSize={maxSizeMB}
                />
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Placeholder para upload
              </label>
              <input
                type="text"
                placeholder="Ex: Arraste arquivos aqui ou clique para selecionar"
                value={placeholder}
                onChange={(e) => setPlaceholder(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        )
      
      case 'text':
        return (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Placeholder
              </label>
              <input
                type="text"
                placeholder="Ex: Digite sua resposta aqui..."
                value={placeholder}
                onChange={(e) => setPlaceholder(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Limite de caracteres
              </label>
              <input
                type="number"
                min="1"
                max="5000"
                value={maxLength}
                onChange={(e) => setMaxLength(parseInt(e.target.value))}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        )

      case 'confirmation':
        return (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Texto de confirmação
              </label>
              <textarea
                placeholder="Ex: Eu confirmo que li e aceito os termos de uso..."
                value={confirmationText}
                onChange={(e) => setConfirmationText(e.target.value)}
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <p className="text-xs text-gray-500 mt-1">
                Este texto será exibido junto com o checkbox de confirmação
              </p>
            </div>
          </div>
        )
      
      default:
        return null
    }
  }

  const selectedType = componentTypes.find(t => t.type === type)

  // Modo de visualização - apenas mostrar informações
  if (isViewMode && component) {
    return (
      <div className="bg-gray-50 border border-gray-200 rounded-lg p-6 space-y-4">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <div className="flex items-center space-x-2">
              {selectedType && (
                <selectedType.icon className={`h-5 w-5 ${selectedType.color}`} />
              )}
              <span className="font-medium text-gray-900">
                {component.title}
              </span>
              {component.isRequired && (
                <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-normal bg-red-50 text-red-600 border border-red-200">
                  Obrigatório
                </span>
              )}
            </div>
          </div>

          <div className="flex items-center space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={onEdit}
              className="text-blue-600 hover:text-blue-700"
              title="Editar componente"
            >
              <Edit3 className="h-4 w-4" />
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={onCancel}
              className="text-gray-600 hover:text-gray-700"
              title="Fechar visualização"
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
        </div>

        {/* Informações do componente */}
        <div className="space-y-3">
          <div>
            <label className="block text-sm font-medium text-gray-700">Tipo</label>
            <p className="text-sm text-gray-900">{selectedType?.label}</p>
          </div>

          {component.description && (
            <div>
              <label className="block text-sm font-medium text-gray-700">Descrição</label>
              <p className="text-sm text-gray-900">{component.description}</p>
            </div>
          )}

          {component.type === 'text' && (
            <>
              {component.placeholder && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Placeholder</label>
                  <p className="text-sm text-gray-900">{component.placeholder}</p>
                </div>
              )}
              {component.maxLength && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Tamanho máximo</label>
                  <p className="text-sm text-gray-900">{component.maxLength} caracteres</p>
                </div>
              )}
            </>
          )}

          {component.type === 'upload' && (
            <>
              {component.placeholder && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Placeholder</label>
                  <p className="text-sm text-gray-900">{component.placeholder}</p>
                </div>
              )}
              {component.maxSizeMB && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Tamanho máximo</label>
                  <p className="text-sm text-gray-900">{component.maxSizeMB} MB</p>
                </div>
              )}
              {component.allowedFileTypes && component.allowedFileTypes.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Tipos de arquivo permitidos</label>
                  <div className="flex flex-wrap gap-1 mt-1">
                    {component.allowedFileTypes.map((fileType) => (
                      <span
                        key={fileType.id}
                        className="inline-flex items-center px-2 py-1 rounded text-xs font-medium bg-blue-50 text-blue-700 border border-blue-200"
                      >
                        {fileType.displayName}
                      </span>
                    ))}
                  </div>
                </div>
              )}
            </>
          )}

          {component.type === 'confirmation' && component.confirmationText && (
            <div>
              <label className="block text-sm font-medium text-gray-700">Texto de confirmação</label>
              <p className="text-sm text-gray-900">{component.confirmationText}</p>
            </div>
          )}
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white border border-gray-200 rounded-lg p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-3">
          <GripVertical className="h-5 w-5 text-gray-400 cursor-move" />
          <div className="flex items-center space-x-2">
            {selectedType && (
              <selectedType.icon className={`h-5 w-5 ${selectedType.color}`} />
            )}
            <span className="font-medium text-gray-900">
              {selectedType?.label || 'Novo Componente'}
            </span>
          </div>
        </div>
        
        <div className="flex items-center space-x-2">
          {onPreview && (
            <Button variant="ghost" size="sm" onClick={onPreview}>
              <Eye className="h-4 w-4" />
            </Button>
          )}
          {onDelete && component && (
            <Button variant="ghost" size="sm" onClick={() => onDelete(component.id)}>
              <Trash2 className="h-4 w-4 text-red-500" />
            </Button>
          )}
        </div>
      </div>

      {/* Type Selection */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-3">
          Tipo do Componente
        </label>
        <div className="grid grid-cols-2 gap-3">
          {componentTypes.map((componentType) => (
            <button
              key={componentType.type}
              type="button"
              onClick={() => setType(componentType.type)}
              className={`p-3 border rounded-lg text-left transition-colors ${
                type === componentType.type
                  ? 'border-blue-500 bg-blue-50'
                  : 'border-gray-200 hover:border-gray-300'
              }`}
            >
              <div className="flex items-center space-x-2 mb-1">
                <componentType.icon className={`h-4 w-4 ${componentType.color}`} />
                <span className="text-sm font-medium">{componentType.label}</span>
              </div>
              <p className="text-xs text-gray-500">{componentType.description}</p>
            </button>
          ))}
        </div>
      </div>

      {/* Basic Info */}
      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Título do Componente *
          </label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Ex: Enviar logo da empresa"
            className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Descrição (opcional)
          </label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Instruções adicionais para o cliente..."
            rows={3}
            className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div className="flex items-center">
          <input
            type="checkbox"
            id="required"
            checked={isRequired}
            onChange={(e) => setIsRequired(e.target.checked)}
            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
          />
          <label htmlFor="required" className="ml-2 text-sm text-gray-700">
            Componente obrigatório
          </label>
        </div>
      </div>

      {/* Advanced Configuration */}
      {(type === 'upload' || type === 'text' || type === 'confirmation') && (
        <div>
          <button
            type="button"
            onClick={() => setShowAdvanced(!showAdvanced)}
            className="flex items-center space-x-2 text-sm text-blue-600 hover:text-blue-700"
          >
            <Settings className="h-4 w-4" />
            <span>Configurações Avançadas</span>
          </button>

          {showAdvanced && (
            <div className="mt-4 p-4 bg-gray-50 rounded-lg">
              {renderTypeSpecificConfig()}
            </div>
          )}
        </div>
      )}

      {/* Actions */}
      <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
        {onCancel && (
          <Button variant="outline" size="sm" onClick={onCancel}>
            Cancelar
          </Button>
        )}
        <Button
          size="sm"
          onClick={handleSave}
          disabled={!title.trim()}
          className="bg-blue-600 hover:bg-blue-700"
        >
          {component ? 'Atualizar Componente' : 'Salvar Componente'}
        </Button>
      </div>
    </div>
  )
}
