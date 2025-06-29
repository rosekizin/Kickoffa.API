import { AxiosError } from 'axios'
import { ApiError } from '@/types'

/**
 * Utilitários para tratamento de erros de API
 */
export class ErrorUtils {
  /**
   * Extrai mensagem de erro de uma resposta de API
   */
  static extractErrorMessage(error: unknown): string {
    console.log('🔍 ErrorUtils: Analisando erro:', error)

    // Se não é um erro do Axios
    if (!(error instanceof Error)) {
      return 'Erro desconhecido'
    }

    // Se é um AxiosError
    if (this.isAxiosError(error)) {
      return this.handleAxiosError(error)
    }

    // Erro genérico
    return error.message || 'Erro desconhecido'
  }

  /**
   * Verifica se é um erro do Axios
   */
  private static isAxiosError(error: Error): error is AxiosError {
    return 'isAxiosError' in error && error.isAxiosError === true
  }

  /**
   * Trata erros específicos do Axios
   */
  private static handleAxiosError(error: AxiosError): string {
    const response = error.response
    const request = error.request

    // Erro de resposta do servidor
    if (response) {
      console.log('📊 ErrorUtils: Status:', response.status)
      console.log('📋 ErrorUtils: Data:', response.data)

      const errorData = response.data as ApiError

      // Tentar extrair mensagem específica
      const specificMessage = this.extractSpecificMessage(errorData)
      if (specificMessage) {
        return specificMessage
      }

      // Mensagens baseadas no status HTTP
      return this.getStatusMessage(response.status)
    }

    // Erro de rede (sem resposta)
    if (request) {
      console.error('❌ ErrorUtils: Erro de rede - sem resposta do servidor')
      return 'Erro de conexão. Verifique sua internet e tente novamente.'
    }

    // Erro na configuração da requisição
    console.error('❌ ErrorUtils: Erro na configuração:', error.message)
    return `Erro na requisição: ${error.message}`
  }

  /**
   * Extrai mensagem específica dos dados de erro
   */
  private static extractSpecificMessage(errorData: any): string | null {
    if (!errorData) return null

    // Mensagem direta
    if (typeof errorData === 'string') {
      return errorData
    }

    // Propriedade message
    if (errorData.message && typeof errorData.message === 'string') {
      return errorData.message
    }

    // Propriedade title
    if (errorData.title && typeof errorData.title === 'string') {
      return errorData.title
    }

    // Erros de validação
    if (errorData.errors) {
      const validationMessages = this.extractValidationErrors(errorData.errors)
      if (validationMessages.length > 0) {
        return validationMessages.join('. ')
      }
    }

    // Propriedade detail (comum em Problem Details)
    if (errorData.detail && typeof errorData.detail === 'string') {
      return errorData.detail
    }

    return null
  }

  /**
   * Extrai mensagens de erros de validação
   */
  private static extractValidationErrors(errors: any): string[] {
    const messages: string[] = []

    if (Array.isArray(errors)) {
      // Array de strings
      messages.push(...errors.filter(e => typeof e === 'string'))
    } else if (typeof errors === 'object') {
      // Objeto com propriedades (ModelState do ASP.NET)
      for (const [field, fieldErrors] of Object.entries(errors)) {
        if (Array.isArray(fieldErrors)) {
          messages.push(...fieldErrors.filter(e => typeof e === 'string'))
        } else if (typeof fieldErrors === 'string') {
          messages.push(fieldErrors)
        }
      }
    }

    return messages
  }

  /**
   * Retorna mensagem baseada no status HTTP
   */
  private static getStatusMessage(status: number): string {
    switch (status) {
      case 400:
        return 'Dados inválidos. Verifique os campos e tente novamente.'
      case 401:
        return 'Credenciais inválidas. Verifique email e senha.'
      case 403:
        return 'Acesso negado. Você não tem permissão para esta ação.'
      case 404:
        return 'Recurso não encontrado.'
      case 409:
        return 'Conflito. O recurso já existe ou está em uso.'
      case 422:
        return 'Dados inválidos. Verifique os campos obrigatórios.'
      case 429:
        return 'Muitas tentativas. Aguarde alguns minutos e tente novamente.'
      case 500:
        return 'Erro interno do servidor. Tente novamente mais tarde.'
      case 502:
        return 'Servidor indisponível. Tente novamente em alguns minutos.'
      case 503:
        return 'Serviço temporariamente indisponível.'
      case 504:
        return 'Timeout do servidor. Tente novamente.'
      default:
        return `Erro do servidor (${status}). Tente novamente mais tarde.`
    }
  }

  /**
   * Verifica se o erro é de validação
   */
  static isValidationError(error: unknown): boolean {
    if (!this.isAxiosError(error as Error)) return false
    
    const axiosError = error as AxiosError
    return axiosError.response?.status === 400 || axiosError.response?.status === 422
  }

  /**
   * Verifica se o erro é de autenticação
   */
  static isAuthError(error: unknown): boolean {
    if (!this.isAxiosError(error as Error)) return false
    
    const axiosError = error as AxiosError
    return axiosError.response?.status === 401
  }

  /**
   * Verifica se o erro é de autorização
   */
  static isAuthorizationError(error: unknown): boolean {
    if (!this.isAxiosError(error as Error)) return false
    
    const axiosError = error as AxiosError
    return axiosError.response?.status === 403
  }
}
