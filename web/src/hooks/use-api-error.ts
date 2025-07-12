import { useToast } from '@/components/providers/toast-provider'
import { parseApiError, formatValidationErrors, type ApiErrorDetails } from '@/lib/api'

/**
 * Hook para tratamento padronizado de erros da API
 */
export const useApiError = () => {
  const { showToast } = useToast()

  /**
   * Processa e exibe um erro da API usando toast
   * @param error - Erro retornado pela API
   * @param customTitle - Título customizado para o toast (opcional)
   */
  const handleApiError = (error: any, customTitle?: string) => {
    const errorDetails = error.apiErrorDetails || parseApiError(error)
    
    // Determinar título e descrição
    const title = customTitle || errorDetails.title
    let description = errorDetails.message

    // Se houver erros de validação, formatá-los
    if (Object.keys(errorDetails.validationErrors).length > 0) {
      const validationMessage = formatValidationErrors(errorDetails.validationErrors)
      description = validationMessage
    }

    // Exibir toast de erro
    showToast({
      type: 'error',
      title,
      description
    })

    // Log para debug
    console.error('🚨 API Error handled:', {
      title,
      description,
      status: errorDetails.status,
      traceId: errorDetails.traceId,
      validationErrors: errorDetails.validationErrors
    })

    return errorDetails
  }

  /**
   * Processa um erro da API sem exibir toast (para tratamento manual)
   * @param error - Erro retornado pela API
   * @returns Detalhes processados do erro
   */
  const parseError = (error: any): ApiErrorDetails => {
    return error.apiErrorDetails || parseApiError(error)
  }

  /**
   * Verifica se um erro é de validação (400 com erros específicos)
   * @param error - Erro retornado pela API
   * @returns true se for erro de validação
   */
  const isValidationError = (error: any): boolean => {
    const errorDetails = error.apiErrorDetails || parseApiError(error)
    return errorDetails.status === 400 && Object.keys(errorDetails.validationErrors).length > 0
  }

  /**
   * Verifica se um erro é de autenticação (401)
   * @param error - Erro retornado pela API
   * @returns true se for erro de autenticação
   */
  const isAuthError = (error: any): boolean => {
    const errorDetails = error.apiErrorDetails || parseApiError(error)
    return errorDetails.status === 401
  }

  /**
   * Verifica se um erro é de autorização (403)
   * @param error - Erro retornado pela API
   * @returns true se for erro de autorização
   */
  const isForbiddenError = (error: any): boolean => {
    const errorDetails = error.apiErrorDetails || parseApiError(error)
    return errorDetails.status === 403
  }

  /**
   * Verifica se um erro é de recurso não encontrado (404)
   * @param error - Erro retornado pela API
   * @returns true se for erro de não encontrado
   */
  const isNotFoundError = (error: any): boolean => {
    const errorDetails = error.apiErrorDetails || parseApiError(error)
    return errorDetails.status === 404
  }

  /**
   * Verifica se um erro é de servidor (5xx)
   * @param error - Erro retornado pela API
   * @returns true se for erro de servidor
   */
  const isServerError = (error: any): boolean => {
    const errorDetails = error.apiErrorDetails || parseApiError(error)
    return errorDetails.status >= 500
  }

  return {
    handleApiError,
    parseError,
    isValidationError,
    isAuthError,
    isForbiddenError,
    isNotFoundError,
    isServerError
  }
}
