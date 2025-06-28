import api from '@/lib/api'
import { User } from '@/types'

export interface UpdateUserProfileRequest {
  name: string
  email: string
  currentPassword?: string
  newPassword?: string
}

export interface UpdateUserProfileResponse {
  id: string
  name: string
  email: string
  role: string
  updatedAt: string
}

/**
 * Serviço responsável por operações de API relacionadas ao perfil do usuário
 */
export class UserService {
  private static readonly BASE_PATH = '/api/user'

  /**
   * Busca o perfil do usuário atual
   */
  static async getCurrentUserProfile(): Promise<User> {
    const response = await api.get<User>(`${this.BASE_PATH}/profile`)
    return response.data
  }

  /**
   * Atualiza o perfil do usuário
   */
  static async updateProfile(profileData: UpdateUserProfileRequest): Promise<UpdateUserProfileResponse> {
    // Validar e limpar dados antes de enviar
    const cleanData = this.cleanProfileData(profileData)
    const response = await api.put<UpdateUserProfileResponse>(`${this.BASE_PATH}/profile`, cleanData)
    return response.data
  }

  /**
   * Verifica se um email já está em uso por outro usuário
   */
  static async checkEmailAvailability(email: string): Promise<{ available: boolean }> {
    const response = await api.get<{ available: boolean }>(`${this.BASE_PATH}/check-email`, {
      params: { email }
    })
    return response.data
  }

  /**
   * Limpa e valida dados do perfil antes de enviar para API
   */
  private static cleanProfileData(data: UpdateUserProfileRequest): UpdateUserProfileRequest {
    const cleanData: UpdateUserProfileRequest = {
      name: data.name?.trim(),
      email: data.email?.trim().toLowerCase()
    }

    // Incluir senhas apenas se fornecidas
    if (data.currentPassword && data.newPassword) {
      cleanData.currentPassword = data.currentPassword
      cleanData.newPassword = data.newPassword
    }

    return cleanData
  }

  /**
   * Valida formato de email
   */
  static validateEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return emailRegex.test(email)
  }

  /**
   * Valida força da senha
   */
  static validatePassword(password: string): { isValid: boolean; errors: string[] } {
    const errors: string[] = []

    if (password.length < 6) {
      errors.push('Senha deve ter pelo menos 6 caracteres')
    }

    if (!/[A-Za-z]/.test(password)) {
      errors.push('Senha deve conter pelo menos uma letra')
    }

    if (!/[0-9]/.test(password)) {
      errors.push('Senha deve conter pelo menos um número')
    }

    return {
      isValid: errors.length === 0,
      errors
    }
  }

  /**
   * Gera sugestões de nome baseado no email
   */
  static generateNameSuggestion(email: string): string {
    if (!email || !this.validateEmail(email)) {
      return ''
    }

    const localPart = email.split('@')[0]
    
    // Remover números e caracteres especiais
    const cleanName = localPart
      .replace(/[0-9._-]/g, ' ')
      .split(' ')
      .filter(part => part.length > 0)
      .map(part => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
      .join(' ')

    return cleanName || localPart
  }

  /**
   * Formata nome para exibição
   */
  static formatDisplayName(name: string): string {
    if (!name) return ''
    
    return name
      .split(' ')
      .filter(part => part.length > 0)
      .map(part => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
      .join(' ')
  }

  /**
   * Obtém iniciais do nome para avatar
   */
  static getInitials(name: string): string {
    if (!name) return '??'
    
    const words = name.trim().split(' ').filter(word => word.length > 0)
    
    if (words.length === 0) return '??'
    if (words.length === 1) return words[0].charAt(0).toUpperCase()
    
    return (words[0].charAt(0) + words[words.length - 1].charAt(0)).toUpperCase()
  }

  /**
   * Verifica se o perfil está completo
   */
  static isProfileComplete(user: Partial<User>): boolean {
    return !!(
      user.name && 
      user.name.trim().length > 0 && 
      user.email && 
      this.validateEmail(user.email)
    )
  }
}
