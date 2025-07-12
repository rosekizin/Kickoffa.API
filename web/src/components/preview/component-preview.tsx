'use client'

import { CheckSquare, Upload, Type, PenTool, Shield, Clock } from 'lucide-react'
import { Button } from '@/components/ui/button'

interface ComponentPreviewProps {
  component: {
    id: number | string
    title: string
    description?: string
    type: 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'
    isRequired?: boolean

    // Propriedades específicas por tipo - seguindo padrão do domínio
    placeholder?: string
    maxLength?: number
    maxSizeMB?: number
    allowedFileTypes?: import('@/types').FileType[]
    confirmationText?: string
  }
}

export const ComponentPreview = ({ component }: ComponentPreviewProps) => {
  const getComponentIcon = (type: string) => {
    switch (type) {
      case 'checkbox':
        return <CheckSquare className="h-5 w-5 text-green-600" />
      case 'upload':
        return <Upload className="h-5 w-5 text-blue-600" />
      case 'text':
        return <Type className="h-5 w-5 text-purple-600" />
      case 'signature':
        return <PenTool className="h-5 w-5 text-orange-600" />
      case 'confirmation':
        return <Shield className="h-5 w-5 text-indigo-600" />
      default:
        return <CheckSquare className="h-5 w-5 text-gray-600" />
    }
  }

  const renderComponentContent = () => {
    switch (component.type) {
      case 'checkbox':
        return (
          <div className="flex items-center space-x-3">
            <input
              type="checkbox"
              disabled
              className="h-5 w-5 text-green-600 border-gray-300 rounded focus:ring-green-500 cursor-not-allowed"
            />
            <span className="text-sm text-gray-700">Marcar como concluído</span>
          </div>
        )

      case 'text':
        return (
          <div className="space-y-3">
            <textarea
              disabled
              placeholder={component.placeholder || 'Digite sua resposta aqui...'}
              rows={3}
              maxLength={component.maxLength}
              className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm bg-gray-50 cursor-not-allowed resize-none"
            />
            {component.maxLength && (
              <p className="text-xs text-gray-500">
                Limite: {component.maxLength} caracteres
              </p>
            )}
          </div>
        )

      case 'signature':
        return (
          <div className="space-y-3">
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center bg-gray-50">
              <PenTool className="h-8 w-8 text-gray-400 mx-auto mb-2" />
              <p className="text-sm text-gray-600 mb-2">Área de assinatura digital</p>
              <p className="text-xs text-gray-500">O cliente poderá assinar aqui usando mouse ou touch</p>
            </div>
            
            {/* Exemplo de assinatura já feita */}
            <div className="bg-green-50 border border-green-200 rounded-lg p-3">
              <div className="flex items-center space-x-3">
                <CheckSquare className="h-5 w-5 text-green-600" />
                <div className="flex-1">
                  <p className="text-sm font-medium text-green-900">Assinatura capturada</p>
                  <p className="text-xs text-green-600">Assinado em {new Date().toLocaleDateString()}</p>
                </div>
              </div>
            </div>
          </div>
        )

      case 'upload':
        return (
          <div className="space-y-3">
            {/* Área de upload */}
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center bg-gray-50">
              <Upload className="h-8 w-8 text-gray-400 mx-auto mb-2" />
              <p className="text-sm text-gray-600 mb-1">
                {component.placeholder || 'Arraste arquivos aqui ou clique para selecionar'}
              </p>
              {component.allowedMimeTypes && (
                <p className="text-xs text-gray-500">
                  Tipos permitidos: {component.allowedMimeTypes}
                </p>
              )}
              {component.maxSizeMB && (
                <p className="text-xs text-gray-500">
                  Tamanho máximo: {component.maxSizeMB}MB por arquivo
                </p>
              )}
            </div>
            
            {/* Arquivo exemplo já "enviado" */}
            <div className="bg-green-50 border border-green-200 rounded-lg p-3">
              <div className="flex items-center space-x-3">
                <div className="flex-shrink-0">
                  <div className="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
                    <Upload className="h-4 w-4 text-green-600" />
                  </div>
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-green-900">
                    exemplo-documento.pdf
                  </p>
                  <p className="text-xs text-green-600">
                    2.3 MB • Enviado com sucesso
                  </p>
                </div>
                <CheckSquare className="h-4 w-4 text-green-600" />
              </div>
            </div>
          </div>
        )

      case 'confirmation':
        return (
          <div className="space-y-3">
            <div className="bg-indigo-50 border border-indigo-200 rounded-lg p-4">
              <p className="text-sm text-indigo-900">
                {component.confirmationText || 'Eu confirmo que li e aceito os termos apresentados.'}
              </p>
            </div>
            
            <div className="flex items-center space-x-3">
              <input
                type="checkbox"
                disabled
                checked
                className="h-5 w-5 text-indigo-600 border-gray-300 rounded focus:ring-indigo-500 cursor-not-allowed"
              />
              <span className="text-sm text-gray-700">Eu confirmo</span>
              <CheckSquare className="h-4 w-4 text-indigo-600" />
            </div>
          </div>
        )

      default:
        return (
          <div className="bg-gray-50 border-2 border-dashed border-gray-300 rounded-lg p-4 text-center">
            <p className="text-sm text-gray-600">
              Tipo de componente não reconhecido: {component.type}
            </p>
          </div>
        )
    }
  }

  return (
    <div className="border border-gray-200 rounded-lg p-4 bg-white">
      <div className="flex items-start space-x-3">
        <div className="flex-shrink-0 mt-1">
          {getComponentIcon(component.type)}
        </div>

        <div className="flex-1 space-y-3">
          <div>
            <h3 className="font-medium text-gray-900 flex items-center">
              {component.title}
              {component.isRequired && (
                <span className="ml-2 text-red-500 text-sm">*</span>
              )}
            </h3>
            {component.description && (
              <p className="text-sm text-gray-600 mt-1">
                {component.description}
              </p>
            )}
          </div>

          {renderComponentContent()}
        </div>
      </div>
    </div>
  )
}
