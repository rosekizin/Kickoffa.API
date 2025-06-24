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
  GripVertical
} from 'lucide-react'
import { Item, CreateItemRequest } from '@/types'

interface ChecklistItemEditorProps {
  item?: Item
  onSave: (item: CreateItemRequest) => void
  onDelete?: (itemId: string) => void
  onPreview?: () => void
  sectionId: string
  order: number
}

type ItemType = 'checkbox' | 'upload' | 'text' | 'signature'

export const ChecklistItemEditor = ({
  item,
  onSave,
  onDelete,
  onPreview,
  sectionId,
  order
}: ChecklistItemEditorProps) => {
  const [title, setTitle] = useState(item?.title || '')
  const [description, setDescription] = useState(item?.description || '')
  const [type, setType] = useState<ItemType>(item?.type || 'checkbox')
  const [isRequired, setIsRequired] = useState(item?.isRequired || false)
  const [config, setConfig] = useState(item?.config || {})
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
    }
  ]

  const handleSave = () => {
    if (!title.trim()) return

    const itemData: CreateItemRequest = {
      sectionId,
      title: title.trim(),
      description: description.trim() || undefined,
      type,
      isRequired,
      order,
      config: Object.keys(config).length > 0 ? config : undefined
    }

    onSave(itemData)
  }

  const updateConfig = (key: string, value: any) => {
    setConfig(prev => ({ ...prev, [key]: value }))
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
              <input
                type="text"
                placeholder="Ex: .pdf,.doc,.jpg,.png"
                value={config.allowedFileTypes?.join(',') || ''}
                onChange={(e) => updateConfig('allowedFileTypes', e.target.value.split(',').map(s => s.trim()))}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <p className="text-xs text-gray-500 mt-1">
                Separar por vírgula. Ex: .pdf,.doc,.jpg
              </p>
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Tamanho máximo por arquivo (MB)
              </label>
              <input
                type="number"
                min="1"
                max="100"
                value={config.maxFileSize ? config.maxFileSize / (1024 * 1024) : 10}
                onChange={(e) => updateConfig('maxFileSize', parseInt(e.target.value) * 1024 * 1024)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Número máximo de arquivos
              </label>
              <input
                type="number"
                min="1"
                max="10"
                value={config.maxFiles || 1}
                onChange={(e) => updateConfig('maxFiles', parseInt(e.target.value))}
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
                value={config.placeholder || ''}
                onChange={(e) => updateConfig('placeholder', e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            
            <div className="flex items-center">
              <input
                type="checkbox"
                id="multiline"
                checked={config.multiline || false}
                onChange={(e) => updateConfig('multiline', e.target.checked)}
                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <label htmlFor="multiline" className="ml-2 text-sm text-gray-700">
                Campo de múltiplas linhas
              </label>
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
      {(type === 'upload' || type === 'text') && (
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
        <Button variant="outline" size="sm">
          Cancelar
        </Button>
        <Button 
          size="sm" 
          onClick={handleSave}
          disabled={!title.trim()}
          className="bg-blue-600 hover:bg-blue-700"
        >
          Salvar Item
        </Button>
      </div>
    </div>
  )
}
