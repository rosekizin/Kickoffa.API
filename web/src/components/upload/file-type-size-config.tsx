'use client'

import { useState, useEffect, useCallback, useMemo } from 'react'
import { FileType } from '@/types'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'

export interface FileTypeSizeConfig {
  fileTypeId: number
  maxSizeMB: number
}

interface FileTypeSizeConfigProps {
  selectedFileTypes: FileType[]
  sizeConfigs: FileTypeSizeConfig[]
  onSizeConfigsChange: (configs: FileTypeSizeConfig[]) => void
  globalMaxSize?: number
}

export const FileTypeSizeConfigComponent = ({
  selectedFileTypes,
  sizeConfigs,
  onSizeConfigsChange,
  globalMaxSize = 10
}: FileTypeSizeConfigProps) => {
  const [localConfigs, setLocalConfigs] = useState<FileTypeSizeConfig[]>(sizeConfigs)

  // Inicializar com sizeConfigs quando disponível
  useEffect(() => {
    if (sizeConfigs.length > 0) {
      setLocalConfigs(sizeConfigs)
    }
  }, [sizeConfigs])

  // Calcular configurações baseadas nos tipos selecionados
  const calculatedConfigs = useMemo(() => {
    return selectedFileTypes.map(fileType => {
      // Primeiro, verificar se existe configuração nos props (dados salvos)
      const savedConfig = sizeConfigs.find(config => config.fileTypeId === fileType.id)
      if (savedConfig) {
        return savedConfig
      }

      // Verificar se já existe configuração local
      const localConfig = localConfigs.find(config => config.fileTypeId === fileType.id)
      if (localConfig) {
        return localConfig
      }

      // Por último, usar tamanho recomendado ou global como padrão
      return {
        fileTypeId: fileType.id,
        maxSizeMB: fileType.recommendedMaxSizeMB || globalMaxSize
      }
    })
  }, [selectedFileTypes, sizeConfigs, localConfigs, globalMaxSize])

  // Sincronizar quando calculatedConfigs mudar
  useEffect(() => {
    // Só atualizar se realmente mudou
    const hasChanged = JSON.stringify(calculatedConfigs) !== JSON.stringify(localConfigs)
    if (hasChanged) {
      setLocalConfigs(calculatedConfigs)
      onSizeConfigsChange(calculatedConfigs)
    }
  }, [calculatedConfigs, localConfigs, onSizeConfigsChange])

  const handleSizeChange = (fileTypeId: number, newSize: number) => {
    const updatedConfigs = localConfigs.map(config =>
      config.fileTypeId === fileTypeId
        ? { ...config, maxSizeMB: newSize }
        : config
    )
    
    setLocalConfigs(updatedConfigs)
    onSizeConfigsChange(updatedConfigs)
  }

  const applyRecommendedSizes = () => {
    const updatedConfigs = localConfigs.map(config => {
      const fileType = selectedFileTypes.find(ft => ft.id === config.fileTypeId)
      return {
        ...config,
        maxSizeMB: fileType?.recommendedMaxSizeMB || globalMaxSize
      }
    })
    
    setLocalConfigs(updatedConfigs)
    onSizeConfigsChange(updatedConfigs)
  }

  const applyGlobalSize = () => {
    const updatedConfigs = localConfigs.map(config => ({
      ...config,
      maxSizeMB: globalMaxSize
    }))
    
    setLocalConfigs(updatedConfigs)
    onSizeConfigsChange(updatedConfigs)
  }

  if (selectedFileTypes.length === 0) {
    return (
      <div className="text-sm text-gray-500 italic">
        Selecione tipos de arquivo para configurar tamanhos específicos
      </div>
    )
  }

  const getCategoryColor = (category: string): string => {
    const colors: Record<string, string> = {
      'Image': 'bg-blue-100 text-blue-800',
      'Document': 'bg-green-100 text-green-800',
      'Audio': 'bg-purple-100 text-purple-800',
      'Video': 'bg-red-100 text-red-800',
      'Code': 'bg-gray-100 text-gray-800',
      'Design': 'bg-pink-100 text-pink-800',
      'Archive': 'bg-yellow-100 text-yellow-800',
      'Spreadsheet': 'bg-emerald-100 text-emerald-800',
      'Presentation': 'bg-orange-100 text-orange-800',
      'Font': 'bg-indigo-100 text-indigo-800'
    }
    return colors[category] || 'bg-gray-100 text-gray-800'
  }

  return (
    <Card>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <CardTitle className="text-sm font-medium">
            Tamanho Máximo por Tipo de Arquivo
          </CardTitle>
          <div className="flex gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={applyRecommendedSizes}
              className="text-xs"
            >
              Usar Recomendados
            </Button>
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={applyGlobalSize}
              className="text-xs"
            >
              Aplicar {globalMaxSize}MB para Todos
            </Button>
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        {selectedFileTypes.map(fileType => {
          const config = localConfigs.find(c => c.fileTypeId === fileType.id)
          const currentSize = config?.maxSizeMB || globalMaxSize
          
          return (
            <div key={fileType.id} className="flex items-center gap-3 p-3 border rounded-lg">
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <span className="font-medium text-sm">{fileType.displayName}</span>
                  <Badge variant="secondary" className={`text-xs ${getCategoryColor(fileType.category)}`}>
                    {fileType.category}
                  </Badge>
                </div>
                <div className="text-xs text-gray-500">
                  {fileType.extension} • {fileType.mimeType}
                  {fileType.recommendedMaxSizeMB && (
                    <span className="ml-2 text-blue-600">
                      (Recomendado: {fileType.recommendedMaxSizeMB}MB)
                    </span>
                  )}
                </div>
              </div>
              
              <div className="flex items-center gap-2">
                <Label htmlFor={`size-${fileType.id}`} className="text-xs whitespace-nowrap">
                  Máx:
                </Label>
                <div className="flex items-center gap-1">
                  <Input
                    id={`size-${fileType.id}`}
                    type="number"
                    min="1"
                    max="1000"
                    value={currentSize}
                    onChange={(e) => handleSizeChange(fileType.id, parseInt(e.target.value) || 1)}
                    className="w-20 h-8 text-sm"
                  />
                  <span className="text-xs text-gray-500">MB</span>
                </div>
              </div>
            </div>
          )
        })}
        
        <div className="text-xs text-gray-500 mt-3 p-2 bg-gray-50 rounded">
          💡 <strong>Dica:</strong> Use os tamanhos recomendados como ponto de partida. 
          Arquivos de texto geralmente precisam de menos espaço, enquanto vídeos e arquivos de design podem precisar de mais.
        </div>
      </CardContent>
    </Card>
  )
}
