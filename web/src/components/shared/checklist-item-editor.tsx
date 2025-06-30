'use client'

import { useState } from 'react'
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
  Shield
} from 'lucide-react'
import { FileTypeSelector } from '@/components/upload/file-type-selector'
import { FileTypeSizeConfigComponent, FileTypeSizeConfig } from '@/components/upload/file-type-size-config'
import { FileType } from '@/types'
import { Item, CreateItemRequest } from '@/types'

interface ChecklistItemEditorProps {
  item?: Item
  onSave: (item: CreateItemRequest) => void
  onDelete?: (itemId: number) => void
  onPreview?: () => void
  onCancel?: () => void
  sectionId: number
  order: number
}

type ItemType = 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'

export const ChecklistItemEditor = ({
  item,
  onSave,
  onDelete,
  onPreview,
  onCancel,
  sectionId,
  order
}: ChecklistItemEditorProps) => {
  const [title, setTitle] = useState(item?.title || '')
  const [description, setDescription] = useState(item?.description || '')
  const [type, setType] = useState<ItemType>(item?.type || 'checkbox')
  const [isRequired, setIsRequired] = useState(item?.isRequired || false)
  const [allowedMimeTypes, setAllowedMimeTypes] = useState(item?.allowedMimeTypes || '')
  const [maxSizeMB, setMaxSizeMB] = useState(item?.maxSizeMB || 10)
  const [placeholder, setPlaceholder] = useState(item?.placeholder || '')
  const [maxLength, setMaxLength] = useState(item?.maxLength || 500)
  const [confirmationText, setConfirmationText] = useState(item?.confirmationText || '')
  const [selectedFileTypes, setSelectedFileTypes] = useState<FileType[]>([])
  const [fileTypeSizeConfigs, setFileTypeSizeConfigs] = useState<FileTypeSizeConfig[]>([])
  const [showAdvanced, setShowAdvanced] = useState(false)

  const itemTypes = [
    {
      type: 'checkbox' as const,
      label: 'Checkbox',
      description: 'Item simples para marcar como concluído',
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
      description: 'Item de confirmação com texto personalizado',
      icon: Shield,
      color: 'text-indigo-600'
    }
  ]

  const handleSave = () => {
    if (!title.trim()) return

    // Para upload, usar selectedFileTypes ao invés de allowedMimeTypes string
    const uploadMimeTypes = type === 'upload' && selectedFileTypes.length > 0
      ? selectedFileTypes.map(ft => ft.mimeType).join(',')
      : allowedMimeTypes

    // Calcular tamanho máximo baseado nas configurações específicas ou usar o padrão
    let calculatedMaxSize = maxSizeMB
    if (type === 'upload' && fileTypeSizeConfigs.length > 0) {
      // Usar o maior tamanho configurado como limite geral
      calculatedMaxSize = Math.max(...fileTypeSizeConfigs.map(config => config.maxSizeMB))
    }

    const itemData: CreateItemRequest = {
      sectionId,
      title: title.trim(),
      description: description.trim() || undefined,
      type,
      isRequired,
      order,
      allowedMimeTypes: type === 'upload' && uploadMimeTypes ? uploadMimeTypes : undefined,
      maxSizeMB: type === 'upload' ? calculatedMaxSize : undefined,
      placeholder: (type === 'text' || type === 'upload') && placeholder ? placeholder : undefined,
      maxLength: type === 'text' ? maxLength : undefined,
      confirmationText: type === 'confirmation' && confirmationText ? confirmationText : undefined,
      // Adicionar configurações específicas de tamanho (para uso futuro)
      fileTypeSizeConfigs: type === 'upload' && fileTypeSizeConfigs.length > 0 ? fileTypeSizeConfigs : undefined
    }

    onSave(itemData)
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

  const selectedType = itemTypes.find(t => t.type === type)

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
              {selectedType?.label || 'Novo Item'}
            </span>
          </div>
        </div>
        
        <div className="flex items-center space-x-2">
          {onPreview && (
            <Button variant="ghost" size="sm" onClick={onPreview}>
              <Eye className="h-4 w-4" />
            </Button>
          )}
          {onDelete && item && (
            <Button variant="ghost" size="sm" onClick={() => onDelete(item.id)}>
              <Trash2 className="h-4 w-4 text-red-500" />
            </Button>
          )}
        </div>
      </div>

      {/* Type Selection */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-3">
          Tipo do Item
        </label>
        <div className="grid grid-cols-2 gap-3">
          {itemTypes.map((itemType) => (
            <button
              key={itemType.type}
              type="button"
              onClick={() => setType(itemType.type)}
              className={`p-3 border rounded-lg text-left transition-colors ${
                type === itemType.type
                  ? 'border-blue-500 bg-blue-50'
                  : 'border-gray-200 hover:border-gray-300'
              }`}
            >
              <div className="flex items-center space-x-2 mb-1">
                <itemType.icon className={`h-4 w-4 ${itemType.color}`} />
                <span className="text-sm font-medium">{itemType.label}</span>
              </div>
              <p className="text-xs text-gray-500">{itemType.description}</p>
            </button>
          ))}
        </div>
      </div>

      {/* Basic Info */}
      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Título do Item *
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
            Item obrigatório
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
          {item ? 'Atualizar Item' : 'Salvar Item'}
        </Button>
      </div>
    </div>
  )
}
