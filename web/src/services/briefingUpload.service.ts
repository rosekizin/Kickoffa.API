import api from '@/lib/api'

export interface BriefingUploadResponse {
  url: string
  fileName: string
  contentType: string
  size: number
}

export interface BriefingUploadProgress {
  loaded: number
  total: number
  percentage: number
}

export class BriefingUploadService {
  /**
   * Upload de imagem para briefing (TipTap)
   */
  static async uploadBriefingImage(
    file: File,
    onProgress?: (progress: BriefingUploadProgress) => void
  ): Promise<BriefingUploadResponse> {
    const formData = new FormData()
    formData.append('file', file)

    try {
      const response = await api.post<BriefingUploadResponse>(
        '/api/fileupload/image/briefing',
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
          onUploadProgress: (progressEvent) => {
            if (onProgress && progressEvent.total) {
              const progress: BriefingUploadProgress = {
                loaded: progressEvent.loaded,
                total: progressEvent.total,
                percentage: Math.round((progressEvent.loaded * 100) / progressEvent.total)
              }
              onProgress(progress)
            }
          }
        }
      )

      return response.data
    } catch (error: any) {
      console.error('Erro no upload de imagem:', error)
      
      // Extrair mensagem de erro da API
      const errorMessage = error.apiErrorDetails?.message || 
                          error.response?.data?.message || 
                          'Erro ao fazer upload da imagem'
      
      throw new Error(errorMessage)
    }
  }

  /**
   * Upload de arquivo genérico para briefing
   */
  static async uploadFile(
    file: File,
    folder: string = 'uploads',
    onProgress?: (progress: BriefingUploadProgress) => void
  ): Promise<BriefingUploadResponse> {
    const formData = new FormData()
    formData.append('file', file)

    try {
      const response = await api.post<BriefingUploadResponse>(
        `/api/fileupload/file?folder=${encodeURIComponent(folder)}`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
          onUploadProgress: (progressEvent) => {
            if (onProgress && progressEvent.total) {
              const progress: BriefingUploadProgress = {
                loaded: progressEvent.loaded,
                total: progressEvent.total,
                percentage: Math.round((progressEvent.loaded * 100) / progressEvent.total)
              }
              onProgress(progress)
            }
          }
        }
      )

      return response.data
    } catch (error: any) {
      console.error('Erro no upload de arquivo:', error)
      
      const errorMessage = error.apiErrorDetails?.message || 
                          error.response?.data?.message || 
                          'Erro ao fazer upload do arquivo'
      
      throw new Error(errorMessage)
    }
  }

  /**
   * Remove um arquivo do S3
   */
  static async deleteFile(fileUrl: string): Promise<void> {
    try {
      await api.delete('/api/fileupload', {
        params: { fileUrl }
      })
    } catch (error: any) {
      console.error('Erro ao remover arquivo:', error)
      
      const errorMessage = error.apiErrorDetails?.message || 
                          error.response?.data?.message || 
                          'Erro ao remover arquivo'
      
      throw new Error(errorMessage)
    }
  }

  /**
   * Valida se o arquivo é uma imagem válida
   */
  static validateImageFile(file: File): void {
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/svg+xml']
    const maxSize = 10 * 1024 * 1024 // 10MB

    if (!allowedTypes.includes(file.type)) {
      throw new Error(`Tipo de arquivo não permitido. Tipos aceitos: ${allowedTypes.join(', ')}`)
    }

    if (file.size > maxSize) {
      throw new Error(`Arquivo muito grande. Tamanho máximo: ${maxSize / (1024 * 1024)}MB`)
    }

    if (file.size === 0) {
      throw new Error('Arquivo está vazio')
    }
  }

  /**
   * Testa se uma URL de imagem está acessível
   */
  static async testImageUrl(url: string): Promise<boolean> {
    try {
      const response = await fetch(url, { 
        method: 'HEAD',
        credentials: 'include' // Incluir cookies de autenticação
      })
      console.log('🔍 Teste de URL:', {
        url,
        status: response.status,
        headers: Object.fromEntries(response.headers.entries())
      })
      return response.ok
    } catch (error) {
      console.error('❌ Erro ao testar URL:', url, error)
      return false
    }
  }

  /**
   * Cria uma URL de preview temporária para o arquivo
   */
  static createPreviewUrl(file: File): string {
    return URL.createObjectURL(file)
  }

  /**
   * Revoga uma URL de preview temporária
   */
  static revokePreviewUrl(url: string): void {
    URL.revokeObjectURL(url)
  }
}
