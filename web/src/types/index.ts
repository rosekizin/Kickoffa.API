// Tipos básicos do domínio Kickoffa
export interface User {
  id: number
  name: string
  email: string
  role: 'freelancer' | 'admin'
  createdAt: string
  updatedAt: string
}

export interface Customer {
  id: number
  firstName: string
  lastName: string
  email?: string
  cpf?: string
  cnpj?: string
  phoneNumber?: string
  address?: string
  createdAt: string
  updatedAt: string
}

export interface CreateCustomerRequest {
  firstName: string
  lastName: string
  email?: string
  cpf?: string
  cnpj?: string
  phoneNumber?: string
  address?: string
}

// Authentication types
export interface LoginRequest {
  email: string
  password: string
  rememberMe?: boolean
}

export interface LoginResponse {
  accessToken: string
  tokenType: string
  expiresIn: number
  refreshToken: string
  userId: number
  email: string
  issuedAt: string | Date
  expiresAt: string | Date
}

export interface AuthUser {
  id: number
  email: string
  isAuthenticated: boolean
}

export interface Checklist {
  id: number
  title: string
  description?: string
  deadline?: string
  slug: string
  accessToken: string
  isPublished: boolean
  ownerId: number
  sections: Section[]
  createdAt: string
  updatedAt: string
}

export interface Section {
  id: number
  checklistId: number
  title: string
  type: 'briefing' | 'checklist'
  order: number

  // Para seções de briefing
  contentJson?: string
  contentHtml?: string
  contentLastUpdated?: string

  // Para seções de checklist
  components?: Component[]

  createdAt: string
  updatedAt: string
}

export interface BriefingMedia {
  id: number
  sectionId: number
  fileName: string
  originalName: string
  mimeType: string
  size: number
  url: string
  createdAt: string
}

export interface Component {
  id: number
  sectionId: number
  title: string
  description?: string
  type: 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'
  isRequired: boolean
  order: number

  // Propriedades específicas por tipo (movidas de config)
  allowedMimeTypes?: string
  maxSizeMB?: number
  placeholder?: string
  maxLength?: number
  confirmationText?: string

  status?: ComponentStatus
  createdAt: string
  updatedAt: string
}

export interface ComponentStatus {
  id: number
  componentId: number
  isCompleted: boolean
  completedAt?: string

  // Dados da resposta
  textResponse?: string
  uploadedFiles?: UploadedFile[]
  signatureData?: string

  createdAt: string
  updatedAt: string
}

export interface UploadedFile {
  id: number
  componentStatusId: number
  fileName: string
  originalName: string
  mimeType: string
  size: number
  url: string
  createdAt: string
}

// Tipos para formulários
export interface CreateChecklistRequest {
  title: string
  description?: string
  deadline?: string
}

export interface CreateSectionRequest {
  checklistId: number
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentJson?: string
  contentHtml?: string
}

export interface FileTypeSizeConfig {
  fileTypeId: number
  maxSizeMB: number
}

export interface CreateComponentRequest {
  sectionId: number
  title: string
  description?: string
  type: 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'
  isRequired: boolean
  order: number
  allowedMimeTypes?: string
  maxSizeMB?: number
  placeholder?: string
  maxLength?: number
  confirmationText?: string
  fileTypeSizeConfigs?: FileTypeSizeConfig[]
}

export interface UpdateComponentStatusRequest {
  componentId: number
  isCompleted: boolean
  textResponse?: string
  signatureData?: string
}

// Tipos para API responses
export interface ApiResponse<T> {
  data: T
  message?: string
  success: boolean
}

export interface PaginatedResponse<T> {
  data: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

// Tipos para FileType
export interface FileType {
  id: number
  mimeType: string
  extension: string
  displayName: string
  description?: string
  category: FileTypeCategory
  recommendedMaxSizeMB?: number
}

export enum FileTypeCategory {
  Image = 'Image',
  Document = 'Document',
  Audio = 'Audio',
  Video = 'Video',
  Archive = 'Archive',
  Code = 'Code',
  Design = 'Design',
  Spreadsheet = 'Spreadsheet',
  Presentation = 'Presentation',
  Font = 'Font',
  Other = 'Other'
}

export interface FileTypesSearchResponse {
  fileTypes: FileType[]
  totalCount: number
}

// Tipos para tratamento de erros
export interface ApiError {
  message?: string
  title?: string
  errors?: Record<string, string[]> | string[]
  status?: number
  type?: string
  traceId?: string
}

export interface ValidationError {
  field: string
  message: string
}

// Tipos para o cliente público (sem autenticação)
export interface PublicChecklistView {
  id: number
  title: string
  description?: string
  deadline?: string
  sections: PublicSectionView[]
  progress: {
    totalComponents: number
    completedComponents: number
    percentage: number
  }
}

export interface PublicSectionView {
  id: number
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentHtml?: string
  components?: PublicComponentView[]
}

export interface PublicComponentView {
  id: number
  title: string
  description?: string
  type: 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'
  isRequired: boolean
  order: number
  isCompleted: boolean
  allowedMimeTypes?: string
  maxSizeMB?: number
  placeholder?: string
  maxLength?: number
  confirmationText?: string
}
