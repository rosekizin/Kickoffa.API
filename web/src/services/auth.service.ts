import api from '@/lib/api'
import { ErrorUtils } from '@/lib/error-utils'
import { LoginRequest, LoginResponse, AuthUser } from '@/types'

/**
 * Serviço responsável por operações de autenticação
 * Usa HttpOnly cookies para segurança contra XSS
 */
export class AuthService {
  private static readonly BASE_PATH = '/api/auth'
  private static readonly USER_KEY = 'kickoffa_user' // Apenas dados do usuário no localStorage

  /**
   * Realiza login do usuário
   * Os tokens são armazenados automaticamente em HttpOnly cookies pelo servidor
   */
  static async login(loginData: LoginRequest): Promise<LoginResponse> {
    console.log('🔐 AuthService: Fazendo requisição de login...')
    const response = await api.post<LoginResponse>(`${this.BASE_PATH}/login`, loginData, {
      withCredentials: true // Importante: permite cookies HttpOnly
    })

    console.log('✅ AuthService: Resposta do login:', response.data)

    // Armazenar apenas dados do usuário (não sensíveis)
    const userData = {
      id: response.data.userId,
      email: response.data.email,
      isAuthenticated: true
    }

    console.log('💾 AuthService: Salvando dados do usuário:', userData)
    this.setUser(userData)

    console.log('✅ AuthService: Login concluído com sucesso')
    return response.data
  }

  /**
   * Realiza logout do usuário
   * O servidor limpa os HttpOnly cookies automaticamente
   */
  static async logout(): Promise<void> {
    try {
      // Chamar endpoint de logout para limpar cookies HttpOnly
      await api.post(`${this.BASE_PATH}/logout`, {}, {
        withCredentials: true // Importante: envia cookies HttpOnly
      })
    } catch (error) {
      // Ignorar erros de logout (token pode estar expirado)
      console.warn('Erro durante logout:', error)
    } finally {
      // Limpar apenas dados do usuário do localStorage
      this.clearAuthData()
    }
  }

  /**
   * Valida se o token atual é válido
   * O token é enviado automaticamente via HttpOnly cookie
   */
  static async validateToken(): Promise<boolean> {
    try {
      console.log('🔍 AuthService: Validando token com servidor...')
      const response = await api.get(`${this.BASE_PATH}/validate`, {
        withCredentials: true // Importante: envia cookies HttpOnly
      })
      console.log('✅ AuthService: Token válido, status =', response.status)
      return response.status === 200
    } catch (error) {
      console.error('❌ AuthService: Erro na validação do token:', error)
      this.clearAuthData()
      return false
    }
  }

  /**
   * Verifica se há dados de usuário (indicando autenticação)
   * Os tokens estão em HttpOnly cookies e não são acessíveis via JavaScript
   */
  static hasUserData(): boolean {
    return this.getUser() !== null
  }

  /**
   * Obtém os dados do usuário armazenados
   */
  static getUser(): AuthUser | null {
    if (typeof window === 'undefined') return null
    
    const userData = localStorage.getItem(this.USER_KEY)
    if (!userData) return null
    
    try {
      return JSON.parse(userData)
    } catch {
      return null
    }
  }

  /**
   * Verifica se o usuário está autenticado
   * Baseado na presença de dados do usuário (tokens estão em HttpOnly cookies)
   */
  static isAuthenticated(): boolean {
    const user = this.getUser()
    const isAuth = !!(user?.isAuthenticated)
    console.log('🔍 AuthService: isAuthenticated =', isAuth, 'user =', user)
    return isAuth
  }



  /**
   * Armazena os dados do usuário
   */
  private static setUser(user: AuthUser): void {
    if (typeof window === 'undefined') return
    
    localStorage.setItem(this.USER_KEY, JSON.stringify(user))
  }

  /**
   * Limpa dados de autenticação do localStorage
   * Os HttpOnly cookies são limpos pelo servidor
   */
  private static clearAuthData(): void {
    if (typeof window === 'undefined') return

    localStorage.removeItem(this.USER_KEY)
  }

  /**
   * Configura interceptors para trabalhar com HttpOnly cookies
   */
  static setupInterceptors(): void {
    // Request interceptor - configurar withCredentials para enviar cookies
    api.interceptors.request.use(
      (config) => {
        // Sempre enviar cookies HttpOnly
        config.withCredentials = true
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor - tratar erros de autenticação
    api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Token expirado ou inválido - cookies serão limpos pelo servidor
          this.clearAuthData()

          // Redirecionar para login se não estiver na página de login
          if (typeof window !== 'undefined' && !window.location.pathname.includes('/auth/login')) {
            window.location.href = '/auth/login'
          }
        }
        return Promise.reject(error)
      }
    )
  }

  /**
   * Formata mensagens de erro de autenticação
   */
  static getErrorMessage(error: any): string {
    return ErrorUtils.extractErrorMessage(error)
  }

  /**
   * Obtém informações do usuário atual
   * (Tokens não são acessíveis pois estão em HttpOnly cookies)
   */
  static getCurrentUser(): AuthUser | null {
    return this.getUser()
  }
}
