import api from '@/lib/api'
import { CustomerUnion, CreateCustomerRequestUnion, CreateNaturalPersonRequest, CreateLegalPersonRequest, CustomerType, NaturalPerson, LegalPerson } from '@/types'

/**
 * Serviço responsável por operações de API e lógica de negócio relacionadas a clientes
 * Usado pelos hooks para manter a separação de responsabilidades
 */
export class CustomerService {
  private static readonly BASE_PATH = '/api/customer'

  /**
   * Cria um novo cliente
   */
  static async createCustomer(customerData: CreateCustomerRequestUnion): Promise<CustomerUnion> {
    // Validar e limpar dados antes de enviar
    const cleanData = this.cleanCustomerData(customerData)
    const response = await api.post<CustomerUnion>(this.BASE_PATH, cleanData)
    return response.data
  }

  /**
   * Busca todos os clientes
   */
  static async getCustomers(): Promise<CustomerUnion[]> {
    const response = await api.get<CustomerUnion[]>(this.BASE_PATH)
    return response.data
  }

  /**
   * Busca um cliente por ID
   */
  static async getCustomerById(id: number): Promise<CustomerUnion> {
    const response = await api.get<CustomerUnion>(`${this.BASE_PATH}/${id}`)
    return response.data
  }

  /**
   * Atualiza um cliente
   */
  static async updateCustomer(id: number, customerData: Partial<CreateCustomerRequestUnion>): Promise<CustomerUnion> {
    const cleanData = this.cleanCustomerData(customerData)
    const response = await api.put<CustomerUnion>(`${this.BASE_PATH}/${id}`, cleanData)
    return response.data
  }

  /**
   * Remove um cliente
   */
  static async deleteCustomer(id: number): Promise<void> {
    await api.delete(`${this.BASE_PATH}/${id}`)
  }

  /**
   * Limpa e valida dados do cliente antes de enviar para API
   */
  private static cleanCustomerData(data: Partial<CreateCustomerRequestUnion>): Partial<CreateCustomerRequestUnion> {
    const baseData = {
      email: data.email?.trim() || undefined,
      phoneNumber: data.phoneNumber?.replace(/\D/g, '') || undefined,
      address: data.address?.trim() || undefined,
      type: data.type
    }

    if (data.type === CustomerType.NaturalPerson) {
      const naturalPersonData = data as Partial<CreateNaturalPersonRequest>
      return {
        ...baseData,
        type: CustomerType.NaturalPerson,
        firstName: naturalPersonData.firstName?.trim(),
        lastName: naturalPersonData.lastName?.trim(),
        cpf: naturalPersonData.cpf?.replace(/\D/g, '') || undefined
      }
    } else if (data.type === CustomerType.LegalCompany) {
      const legalPersonData = data as Partial<CreateLegalPersonRequest>
      return {
        ...baseData,
        type: CustomerType.LegalCompany,
        company: legalPersonData.company?.trim(),
        cnpj: legalPersonData.cnpj?.replace(/\D/g, '') || undefined
      }
    }

    return baseData
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

  /**
   * Verifica se um customer é pessoa física
   */
  static isNaturalPerson(customer: CustomerUnion): customer is NaturalPerson {
    return customer.type === CustomerType.NaturalPerson
  }

  /**
   * Verifica se um customer é pessoa jurídica
   */
  static isLegalPerson(customer: CustomerUnion): customer is LegalPerson {
    return customer.type === CustomerType.LegalCompany
  }

  /**
   * Obtém o nome de exibição do customer
   */
  static getDisplayName(customer: CustomerUnion): string {
    if (this.isNaturalPerson(customer)) {
      return `${customer.firstName} ${customer.lastName}`
    } else if (this.isLegalPerson(customer)) {
      return customer.company
    }
    return 'Cliente'
  }

  /**
   * Obtém o documento do customer (CPF ou CNPJ)
   */
  static getDocument(customer: CustomerUnion): string | undefined {
    if (this.isNaturalPerson(customer)) {
      return customer.cpf
    } else if (this.isLegalPerson(customer)) {
      return customer.cnpj
    }
    return undefined
  }

  /**
   * Obtém o documento formatado do customer
   */
  static getFormattedDocument(customer: CustomerUnion): string | undefined {
    if (this.isNaturalPerson(customer) && customer.cpf) {
      return this.formatCpf(customer.cpf)
    } else if (this.isLegalPerson(customer) && customer.cnpj) {
      return this.formatCnpj(customer.cnpj)
    }
    return undefined
  }

  /**
   * Obtém o tipo de customer como string
   */
  static getCustomerTypeLabel(customer: CustomerUnion): string {
    return customer.type === CustomerType.NaturalPerson ? 'Pessoa Física' : 'Pessoa Jurídica'
  }

  /**
   * Verifica se um customer corresponde ao termo de busca
   * Busca em FirstName, LastName, Company, email e documentos
   */
  static matchesSearchTerm(customer: CustomerUnion, searchTerm: string): boolean {
    if (!searchTerm.trim()) return true

    const searchLower = searchTerm.toLowerCase()

    // Buscar no nome de exibição (FirstName + LastName ou Company)
    const displayName = this.getDisplayName(customer).toLowerCase()
    if (displayName.includes(searchLower)) return true

    // Buscar no email
    if (customer.email?.toLowerCase().includes(searchLower)) return true

    // Buscar no documento (CPF ou CNPJ)
    const document = this.getDocument(customer)
    if (document?.includes(searchTerm)) return true

    // Buscar especificamente em cada campo para maior precisão
    if (this.isNaturalPerson(customer)) {
      if (customer.firstName.toLowerCase().includes(searchLower)) return true
      if (customer.lastName.toLowerCase().includes(searchLower)) return true
    } else if (this.isLegalPerson(customer)) {
      if (customer.company.toLowerCase().includes(searchLower)) return true
    }

    return false
  }

  /**
   * Filtra uma lista de customers baseado no termo de busca
   */
  static filterBySearchTerm(customers: CustomerUnion[], searchTerm: string): CustomerUnion[] {
    if (!searchTerm.trim()) return customers

    return customers.filter(customer => this.matchesSearchTerm(customer, searchTerm))
  }
}
