import { useState, useCallback } from 'react'
import { FileUploadService, FileUploadResponse, UploadProgress } from '@/services/fileUpload.service'

export interface UploadState {
  isUploading: boolean
  progress: number
  error: string | null
  uploadedFile: FileUploadResponse | null
}

export interface UseImageUploadReturn {
  uploadState: UploadState
  uploadImage: (file: File) => Promise<FileUploadResponse | null>
  resetUpload: () => void
  clearError: () => void
}

export const useImageUpload = (): UseImageUploadReturn => {
  const [uploadState, setUploadState] = useState<UploadState>({
    isUploading: false,
    progress: 0,
    error: null,
    uploadedFile: null
  })

  const uploadImage = useCallback(async (file: File): Promise<FileUploadResponse | null> => {
    try {
      // Validar arquivo antes do upload
      FileUploadService.validateImageFile(file)

      // Resetar estado
      setUploadState({
        isUploading: true,
        progress: 0,
        error: null,
        uploadedFile: null
      })

      // Fazer upload com callback de progresso
      const result = await FileUploadService.uploadBriefingImage(
        file,
        (progress: UploadProgress) => {
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
export interface MultiUploadState {
  [fileId: string]: UploadState
}

export interface UseMultiImageUploadReturn {
  uploadStates: MultiUploadState
  uploadImage: (file: File, fileId?: string) => Promise<FileUploadResponse | null>
  removeUpload: (fileId: string) => void
  clearAllErrors: () => void
  resetAll: () => void
}

export const useMultiImageUpload = (): UseMultiImageUploadReturn => {
  const [uploadStates, setUploadStates] = useState<MultiUploadState>({})

  const uploadImage = useCallback(async (file: File, fileId?: string): Promise<FileUploadResponse | null> => {
    const id = fileId || `upload_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

    try {
      // Validar arquivo
      FileUploadService.validateImageFile(file)

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
      const result = await FileUploadService.uploadBriefingImage(
        file,
        (progress: UploadProgress) => {
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
