'use client'

import { useState, useEffect, useMemo } from 'react'
import { Check, ChevronDown, Search, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { useSearchFileTypes } from '@/hooks/use-api'
import { FileType, FileTypeCategory } from '@/types'

interface FileTypeSelectorProps {
  selectedFileTypes: FileType[]
  onSelectionChange: (fileTypes: FileType[]) => void
  placeholder?: string
  maxSelections?: number
}

export const FileTypeSelector = ({
  selectedFileTypes,
  onSelectionChange,
  placeholder = "Buscar tipos de arquivo...",
  maxSelections
}: FileTypeSelectorProps) => {
  const [isOpen, setIsOpen] = useState(false)
  const [searchTerm, setSearchTerm] = useState('')
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('')

  // Debounce search term
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm)
    }, 300)

    return () => clearTimeout(timer)
  }, [searchTerm])

  const { data: searchResponse, isLoading } = useSearchFileTypes(debouncedSearchTerm)

  const availableFileTypes = useMemo(() => {
    return searchResponse?.fileTypes || []
  }, [searchResponse])

  const groupedFileTypes = useMemo(() => {
    const groups: Record<FileTypeCategory, FileType[]> = {} as Record<FileTypeCategory, FileType[]>
    
    availableFileTypes.forEach(fileType => {
      if (!groups[fileType.category]) {
        groups[fileType.category] = []
      }
      groups[fileType.category].push(fileType)
    })

    return groups
  }, [availableFileTypes])

  const handleToggleFileType = (fileType: FileType) => {
    const isSelected = selectedFileTypes.some(ft => ft.id === fileType.id)
    
    if (isSelected) {
      // Remove from selection
      onSelectionChange(selectedFileTypes.filter(ft => ft.id !== fileType.id))
    } else {
      // Add to selection (check max limit)
      if (maxSelections && selectedFileTypes.length >= maxSelections) {
        return // Don't add if max reached
      }
      onSelectionChange([...selectedFileTypes, fileType])
    }
  }

  const handleRemoveFileType = (fileTypeId: number) => {
    onSelectionChange(selectedFileTypes.filter(ft => ft.id !== fileTypeId))
  }

  const getCategoryDisplayName = (category: FileTypeCategory): string => {
    const categoryNames: Record<FileTypeCategory, string> = {
      [FileTypeCategory.Image]: 'Imagens',
      [FileTypeCategory.Document]: 'Documentos',
      [FileTypeCategory.Audio]: 'Áudio',
      [FileTypeCategory.Video]: 'Vídeo',
      [FileTypeCategory.Archive]: 'Arquivos Comprimidos',
      [FileTypeCategory.Code]: 'Código',
      [FileTypeCategory.Design]: 'Design',
      [FileTypeCategory.Spreadsheet]: 'Planilhas',
      [FileTypeCategory.Presentation]: 'Apresentações',
      [FileTypeCategory.Font]: 'Fontes',
      [FileTypeCategory.Other]: 'Outros'
    }
    return categoryNames[category] || category
  }

  return (
    <div className="relative">
      {/* Selected file types */}
      {selectedFileTypes.length > 0 && (
        <div className="mb-3 flex flex-wrap gap-2">
          {selectedFileTypes.map(fileType => (
            <div
              key={fileType.id}
              className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded-md"
            >
              <span>{fileType.displayName}</span>
              <button
                type="button"
                onClick={() => handleRemoveFileType(fileType.id)}
                className="hover:bg-blue-200 rounded p-0.5"
              >
                <X className="h-3 w-3" />
              </button>
            </div>
          ))}
        </div>
      )}

      {/* Dropdown trigger */}
      <div className="relative">
        <button
          type="button"
          onClick={() => setIsOpen(!isOpen)}
          className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm text-left bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <div className="flex items-center justify-between">
            <span className="text-gray-500">
              {selectedFileTypes.length > 0 
                ? `${selectedFileTypes.length} tipo(s) selecionado(s)`
                : placeholder
              }
            </span>
            <ChevronDown className={`h-4 w-4 text-gray-400 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
          </div>
        </button>

        {/* Dropdown content */}
        {isOpen && (
          <div className="absolute z-50 w-full mt-1 bg-white border border-gray-300 rounded-md shadow-lg max-h-80 overflow-hidden">
            {/* Search input */}
            <div className="p-3 border-b border-gray-200">
              <div className="relative">
                <Search className="h-4 w-4 absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar tipos de arquivo..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
            </div>

            {/* File types list */}
            <div className="max-h-60 overflow-y-auto">
              {isLoading ? (
                <div className="p-4 text-center text-gray-500">
                  Carregando tipos de arquivo...
                </div>
              ) : Object.keys(groupedFileTypes).length === 0 ? (
                <div className="p-4 text-center text-gray-500">
                  Nenhum tipo de arquivo encontrado
                </div>
              ) : (
                Object.entries(groupedFileTypes).map(([category, fileTypes]) => (
                  <div key={category} className="border-b border-gray-100 last:border-b-0">
                    <div className="px-3 py-2 bg-gray-50 text-xs font-medium text-gray-700 uppercase tracking-wide">
                      {getCategoryDisplayName(category as FileTypeCategory)}
                    </div>
                    {fileTypes.map(fileType => {
                      const isSelected = selectedFileTypes.some(ft => ft.id === fileType.id)
                      const isDisabled = maxSelections && !isSelected && selectedFileTypes.length >= maxSelections

                      return (
                        <button
                          key={fileType.id}
                          type="button"
                          onClick={() => !isDisabled && handleToggleFileType(fileType)}
                          disabled={isDisabled}
                          className={`w-full px-3 py-2 text-left text-sm hover:bg-gray-50 focus:outline-none focus:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed ${
                            isSelected ? 'bg-blue-50 text-blue-900' : 'text-gray-900'
                          }`}
                        >
                          <div className="flex items-center justify-between">
                            <div className="flex-1">
                              <div className="font-medium">{fileType.displayName}</div>
                              <div className="text-xs text-gray-500">
                                {fileType.extension} • {fileType.mimeType}
                                {fileType.recommendedMaxSizeMB && (
                                  <span> • Max: {fileType.recommendedMaxSizeMB}MB</span>
                                )}
                              </div>
                              {fileType.description && (
                                <div className="text-xs text-gray-400 mt-1">
                                  {fileType.description}
                                </div>
                              )}
                            </div>
                            {isSelected && (
                              <Check className="h-4 w-4 text-blue-600 flex-shrink-0" />
                            )}
                          </div>
                        </button>
                      )
                    })}
                  </div>
                ))
              )}
            </div>

            {/* Footer */}
            <div className="p-3 border-t border-gray-200 bg-gray-50">
              <div className="flex items-center justify-between text-xs text-gray-500">
                <span>
                  {selectedFileTypes.length} selecionado(s)
                  {maxSelections && ` de ${maxSelections} máximo`}
                </span>
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  onClick={() => setIsOpen(false)}
                  className="text-xs h-6 px-2"
                >
                  Fechar
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
