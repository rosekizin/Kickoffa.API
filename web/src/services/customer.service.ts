import api from '@/lib/api'
import { Customer, CreateCustomerRequest } from '@/types'

/**
 * Serviço responsável por operações de API e lógica de negócio relacionadas a clientes
 * Usado pelos hooks para manter a separação de responsabilidades
 */
export class CustomerService {
  private static readonly BASE_PATH = '/api/customer'

  /**
   * Cria um novo cliente
   */
  static async createCustomer(customerData: CreateCustomerRequest): Promise<Customer> {
    // Validar e limpar dados antes de enviar
    const cleanData = this.cleanCustomerData(customerData)
    const response = await api.post<Customer>(this.BASE_PATH, cleanData)
    return response.data
  }

  /**
   * Busca todos os clientes
   */
  static async getCustomers(): Promise<Customer[]> {
    const response = await api.get<Customer[]>(this.BASE_PATH)
    return response.data
  }

  /**
   * Busca um cliente por ID
   */
  static async getCustomerById(id: string): Promise<Customer> {
    const response = await api.get<Customer>(`${this.BASE_PATH}/${id}`)
    return response.data
  }

  /**
   * Atualiza um cliente
   */
  static async updateCustomer(id: string, customerData: Partial<CreateCustomerRequest>): Promise<Customer> {
    const cleanData = this.cleanCustomerData(customerData)
    const response = await api.put<Customer>(`${this.BASE_PATH}/${id}`, cleanData)
    return response.data
  }

  /**
   * Remove um cliente
   */
  static async deleteCustomer(id: string): Promise<void> {
    await api.delete(`${this.BASE_PATH}/${id}`)
  }

  /**
   * Limpa e valida dados do cliente antes de enviar para API
   */
  private static cleanCustomerData(data: Partial<CreateCustomerRequest>): Partial<CreateCustomerRequest> {
    return {
      firstName: data.firstName?.trim(),
      lastName: data.lastName?.trim(),
      email: data.email?.trim() || undefined,
      phoneNumber: data.phoneNumber?.replace(/\D/g, '') || undefined,
      address: data.address?.trim() || undefined,
      cpf: data.cpf?.replace(/\D/g, '') || undefined,
      cnpj: data.cnpj?.replace(/\D/g, '') || undefined
    }
  }

  /**
   * Valida CPF
   * @param cpf CPF a ser validado (apenas números)
   * @returns true se válido
   */
  static validateCpf(cpf: string): boolean {
    // Remove caracteres não numéricos
    const cleanCpf = cpf.replace(/\D/g, '')
    
    if (cleanCpf.length !== 11) return false
    
    // Verifica se todos os dígitos são iguais
    if (/^(\d)\1{10}$/.test(cleanCpf)) return false
    
    // Validação do primeiro dígito verificador
    let sum = 0
    for (let i = 0; i < 9; i++) {
      sum += parseInt(cleanCpf.charAt(i)) * (10 - i)
    }
    let remainder = (sum * 10) % 11
    if (remainder === 10 || remainder === 11) remainder = 0
    if (remainder !== parseInt(cleanCpf.charAt(9))) return false
    
    // Validação do segundo dígito verificador
    sum = 0
    for (let i = 0; i < 10; i++) {
      sum += parseInt(cleanCpf.charAt(i)) * (11 - i)
    }
    remainder = (sum * 10) % 11
    if (remainder === 10 || remainder === 11) remainder = 0
    if (remainder !== parseInt(cleanCpf.charAt(10))) return false
    
    return true
  }

  /**
   * Valida CNPJ
   * @param cnpj CNPJ a ser validado (apenas números)
   * @returns true se válido
   */
  static validateCnpj(cnpj: string): boolean {
    // Remove caracteres não numéricos
    const cleanCnpj = cnpj.replace(/\D/g, '')
    
    if (cleanCnpj.length !== 14) return false
    
    // Verifica se todos os dígitos são iguais
    if (/^(\d)\1{13}$/.test(cleanCnpj)) return false
    
    // Validação do primeiro dígito verificador
    let sum = 0
    let weight = 2
    for (let i = 11; i >= 0; i--) {
      sum += parseInt(cleanCnpj.charAt(i)) * weight
      weight = weight === 9 ? 2 : weight + 1
    }
    let remainder = sum % 11
    const firstDigit = remainder < 2 ? 0 : 11 - remainder
    if (firstDigit !== parseInt(cleanCnpj.charAt(12))) return false
    
    // Validação do segundo dígito verificador
    sum = 0
    weight = 2
    for (let i = 12; i >= 0; i--) {
      sum += parseInt(cleanCnpj.charAt(i)) * weight
      weight = weight === 9 ? 2 : weight + 1
    }
    remainder = sum % 11
    const secondDigit = remainder < 2 ? 0 : 11 - remainder
    if (secondDigit !== parseInt(cleanCnpj.charAt(13))) return false
    
    return true
  }

  /**
   * Formata CPF para exibição
   * @param cpf CPF a ser formatado
   * @returns CPF formatado (xxx.xxx.xxx-xx)
   */
  static formatCpf(cpf: string): string {
    const cleanCpf = cpf.replace(/\D/g, '')
    return cleanCpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
  }

  /**
   * Formata CNPJ para exibição
   * @param cnpj CNPJ a ser formatado
   * @returns CNPJ formatado (xx.xxx.xxx/xxxx-xx)
   */
  static formatCnpj(cnpj: string): string {
    const cleanCnpj = cnpj.replace(/\D/g, '')
    return cleanCnpj.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5')
  }

  /**
   * Formata telefone para exibição
   * @param phone Telefone a ser formatado
   * @returns Telefone formatado
   */
  static formatPhone(phone: string): string {
    const cleanPhone = phone.replace(/\D/g, '')
    
    if (cleanPhone.length === 11) {
      return cleanPhone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
    } else if (cleanPhone.length === 10) {
      return cleanPhone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3')
    }
    
    return phone
  }
}
