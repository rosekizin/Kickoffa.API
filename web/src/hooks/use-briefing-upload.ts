import { useState, useCallback } from 'react'
import { BriefingUploadService, BriefingUploadResponse, BriefingUploadProgress } from '@/services/briefingUpload.service'

export interface BriefingUploadState {
  isUploading: boolean
  progress: number
  error: string | null
  uploadedFile: BriefingUploadResponse | null
}

export interface UseBriefingUploadReturn {
  uploadState: BriefingUploadState
  uploadImage: (file: File) => Promise<BriefingUploadResponse | null>
  resetUpload: () => void
  clearError: () => void
}

export const useBriefingUpload = (): UseBriefingUploadReturn => {
  const [uploadState, setUploadState] = useState<BriefingUploadState>({
    isUploading: false,
    progress: 0,
    error: null,
    uploadedFile: null
  })

  const uploadImage = useCallback(async (file: File): Promise<BriefingUploadResponse | null> => {
    try {
      // Validar arquivo antes do upload
      BriefingUploadService.validateImageFile(file)

      // Resetar estado
      setUploadState({
        isUploading: true,
        progress: 0,
        error: null,
        uploadedFile: null
      })

      // Fazer upload com callback de progresso
      const result = await BriefingUploadService.uploadBriefingImage(
        file,
        (progress: BriefingUploadProgress) => {
          setUploadState(prev => ({
            ...prev,
            progress: progress.percentage
          }))
        }
      )

      // Upload concluído com sucesso
      setUploadState({
        isUploading: false,
        progress: 100,
        error: null,
        uploadedFile: result
      })

      return result

    } catch (error: any) {
      // Upload falhou
      setUploadState({
        isUploading: false,
        progress: 0,
        error: error.message || 'Erro ao fazer upload da imagem',
        uploadedFile: null
      })

      return null
    }
  }, [])

  const resetUpload = useCallback(() => {
    setUploadState({
      isUploading: false,
      progress: 0,
      error: null,
      uploadedFile: null
    })
  }, [])

  const clearError = useCallback(() => {
    setUploadState(prev => ({
      ...prev,
      error: null
    }))
  }, [])

  return {
    uploadState,
    uploadImage,
    resetUpload,
    clearError
  }
}

// Hook para múltiplos uploads simultâneos
export interface MultiBriefingUploadState {
  [fileId: string]: BriefingUploadState
}

export interface UseMultiBriefingUploadReturn {
  uploadStates: MultiBriefingUploadState
  uploadImage: (file: File, fileId?: string) => Promise<BriefingUploadResponse | null>
  removeUpload: (fileId: string) => void
  clearAllErrors: () => void
  resetAll: () => void
}

export const useMultiBriefingUpload = (): UseMultiBriefingUploadReturn => {
  const [uploadStates, setUploadStates] = useState<MultiBriefingUploadState>({})

  const uploadImage = useCallback(async (file: File, fileId?: string): Promise<BriefingUploadResponse | null> => {
    const id = fileId || `upload_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

    try {
      // Validar arquivo
      BriefingUploadService.validateImageFile(file)

      // Inicializar estado do upload
      setUploadStates(prev => ({
        ...prev,
        [id]: {
          isUploading: true,
          progress: 0,
          error: null,
          uploadedFile: null
        }
      }))

      // Fazer upload
      const result = await BriefingUploadService.uploadBriefingImage(
        file,
        (progress: BriefingUploadProgress) => {
          setUploadStates(prev => ({
            ...prev,
            [id]: {
              ...prev[id],
              progress: progress.percentage
            }
          }))
        }
      )

      // Upload concluído
      setUploadStates(prev => ({
        ...prev,
        [id]: {
          isUploading: false,
          progress: 100,
          error: null,
          uploadedFile: result
        }
      }))

      return result

    } catch (error: any) {
      // Upload falhou
      setUploadStates(prev => ({
        ...prev,
        [id]: {
          isUploading: false,
          progress: 0,
          error: error.message || 'Erro ao fazer upload da imagem',
          uploadedFile: null
        }
      }))

      return null
    }
  }, [])

  const removeUpload = useCallback((fileId: string) => {
    setUploadStates(prev => {
      const newStates = { ...prev }
      delete newStates[fileId]
      return newStates
    })
  }, [])

  const clearAllErrors = useCallback(() => {
    setUploadStates(prev => {
      const newStates = { ...prev }
      Object.keys(newStates).forEach(id => {
        if (newStates[id].error) {
          newStates[id] = {
            ...newStates[id],
            error: null
          }
        }
      })
      return newStates
    })
  }, [])

  const resetAll = useCallback(() => {
    setUploadStates({})
  }, [])

  return {
    uploadStates,
    uploadImage,
    removeUpload,
    clearAllErrors,
    resetAll
  }
}
