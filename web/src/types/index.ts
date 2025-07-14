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

  // Propriedades específicas por tipo - seguindo padrão do domínio

  // Para TextComponent
  placeholder?: string
  maxLength?: number

  // Para UploadComponent
  allowedFileTypes?: FileType[]
  fileTypeSizeConfigs?: FileTypeSizeConfig[]
  componentFiles?: UploadComponentFile[]

  // Para ConfirmationComponent
  confirmationText?: string

  status?: ComponentStatus
  createdAt: string
  updatedAt: string
}

export interface UploadComponentFile {
  id: number
  componentId: number
  fileName: string
  originalName: string
  mimeType: string
  size: number
  url: string
  createdAt: string
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
  id?: number // ID para updates (0 ou undefined para criações)
  title: string
  description?: string
  deadline?: string
  sections: CreateSectionRequest[]
}

export interface CreateSectionRequest {
  id?: number // ID para updates (0 ou undefined para criações)
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentJson?: string
  contentHtml?: string
  components?: CreateComponentRequest[]
}

// Interfaces base para componentes
export interface CreateComponentBaseRequest {
  id?: number // ID para updates (0 ou undefined para criações)
  title: string
  description?: string
  isRequired: boolean
  order: number
}

export interface CreateCheckboxComponentRequest extends CreateComponentBaseRequest {
  type: 'checkbox'
}

export interface CreateTextComponentRequest extends CreateComponentBaseRequest {
  type: 'text'
  placeholder?: string
  maxLength?: number
}

export interface CreateUploadComponentRequest extends CreateComponentBaseRequest {
  type: 'upload'
  placeholder?: string
  allowedFileTypeIds: number[]
  allowedFileTypes?: FileType[]
  fileTypeSizeConfigs?: FileTypeSizeConfig[]
}

export interface CreateSignatureComponentRequest extends CreateComponentBaseRequest {
  type: 'signature'
}

export interface CreateConfirmationComponentRequest extends CreateComponentBaseRequest {
  type: 'confirmation'
  confirmationText?: string
}

export type CreateComponentRequest =
  | CreateCheckboxComponentRequest
  | CreateTextComponentRequest
  | CreateUploadComponentRequest
  | CreateSignatureComponentRequest
  | CreateConfirmationComponentRequest

export interface FileTypeSizeConfig {
  fileTypeId: number
  maxSizeMB: number
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

  // Propriedades específicas por tipo - seguindo padrão do domínio
  placeholder?: string
  maxLength?: number
  allowedFileTypes?: FileType[]
  confirmationText?: string
}
