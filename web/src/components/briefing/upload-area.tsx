'use client'

import { useCallback, useState } from 'react'
import { Upload, Image as ImageIcon, FileText, AlertCircle } from 'lucide-react'
import { useBriefingUpload } from '@/hooks/use-briefing-upload'

interface UploadAreaProps {
  onImageUploaded?: (url: string, fileName: string) => void
  className?: string
  maxSizeMB?: number
  allowedTypes?: string[]
}

export const UploadArea = ({ 
  onImageUploaded, 
  className = '',
  maxSizeMB = 10,
  allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/svg+xml']
}: UploadAreaProps) => {
  const [isDragOver, setIsDragOver] = useState(false)
  const { uploadState, uploadImage, clearError } = useBriefingUpload()

  const handleFileSelect = useCallback(async (files: FileList | null) => {
    if (!files || files.length === 0) return

    const file = files[0]
    
    try {
      const result = await uploadImage(file)
      if (result) {
        onImageUploaded?.(result.url, result.fileName)
      }
    } catch (error) {
      console.error('Erro no upload:', error)
    }
  }, [uploadImage, onImageUploaded])

  const handleClick = useCallback(() => {
    const input = document.createElement('input')
    input.type = 'file'
    input.accept = allowedTypes.join(',')
    input.onchange = (e) => {
      const target = e.target as HTMLInputElement
      handleFileSelect(target.files)
    }
    input.click()
  }, [allowedTypes, handleFileSelect])

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    setIsDragOver(true)
  }, [])

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    setIsDragOver(false)
  }, [])

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    setIsDragOver(false)
    
    const files = e.dataTransfer.files
    handleFileSelect(files)
  }, [handleFileSelect])

  const formatFileTypes = () => {
    return allowedTypes
      .map(type => type.split('/')[1].toUpperCase())
      .join(', ')
  }

  return (
    <div className={`relative ${className}`}>
      {/* Área de Upload */}
      <div
        onClick={handleClick}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        className={`
          relative border-2 border-dashed rounded-lg p-8 text-center cursor-pointer
          transition-all duration-200 ease-in-out
          ${isDragOver 
            ? 'border-blue-400 bg-blue-50 scale-[1.02]' 
            : 'border-gray-300 hover:border-gray-400 hover:bg-gray-50'
          }
          ${uploadState.isUploading ? 'pointer-events-none opacity-75' : ''}
        `}
      >
        {/* Ícone e Texto Principal */}
        <div className="flex flex-col items-center space-y-4">
          {uploadState.isUploading ? (
            <div className="flex flex-col items-center space-y-2">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
              <p className="text-sm font-medium text-blue-600">
                Fazendo upload... {uploadState.progress}%
              </p>
            </div>
          ) : (
            <>
              <div className={`
                p-3 rounded-full transition-colors duration-200
                ${isDragOver ? 'bg-blue-100' : 'bg-gray-100'}
              `}>
                {isDragOver ? (
                  <Upload className="h-8 w-8 text-blue-500" />
                ) : (
                  <ImageIcon className="h-8 w-8 text-gray-400" />
                )}
              </div>
              
              <div className="space-y-2">
                <p className="text-lg font-medium text-gray-700">
                  {isDragOver ? (
                    'Solte a imagem aqui'
                  ) : (
                    <>
                      <span className="text-blue-600 hover:text-blue-700 underline">
                        Clique para fazer upload
                      </span>
                      {' ou arraste e solte'}
                    </>
                  )}
                </p>
                
                <p className="text-sm text-gray-500">
                  Máximo {maxSizeMB}MB • {formatFileTypes()}
                </p>
              </div>
            </>
          )}
        </div>

        {/* Barra de Progresso */}
        {uploadState.isUploading && uploadState.progress > 0 && (
          <div className="absolute bottom-4 left-4 right-4">
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div 
                className="bg-blue-500 h-2 rounded-full transition-all duration-300"
                style={{ width: `${uploadState.progress}%` }}
              />
            </div>
          </div>
        )}
      </div>

      {/* Mensagem de Erro */}
      {uploadState.error && (
        <div className="mt-3 p-3 bg-red-50 border border-red-200 rounded-lg flex items-start space-x-2">
          <AlertCircle className="h-5 w-5 text-red-500 flex-shrink-0 mt-0.5" />
          <div className="flex-1">
            <p className="text-sm text-red-700">{uploadState.error}</p>
            <button
              onClick={clearError}
              className="text-xs text-red-600 hover:text-red-800 underline mt-1"
            >
              Fechar
            </button>
          </div>
        </div>
      )}

      {/* Overlay de Drag */}
      {isDragOver && (
        <div className="absolute inset-0 bg-blue-500 bg-opacity-10 rounded-lg pointer-events-none" />
      )}
    </div>
  )
}
