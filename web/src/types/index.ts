// Tipos básicos do domínio Kickoffa
export interface User {
  id: string
  name: string
  email: string
  role: 'freelancer' | 'admin'
  createdAt: string
  updatedAt: string
}

export interface Checklist {
  id: string
  title: string
  description?: string
  deadline?: string
  token: string
  isPublished: boolean
  sections: Section[]
  createdAt: string
  updatedAt: string
  createdBy: string
}

export interface Section {
  id: string
  checklistId: string
  title: string
  type: 'briefing' | 'checklist'
  order: number

  // Para seções de briefing
  contentJson?: string
  contentHtml?: string

  // Para seções de checklist
  items?: Item[]

  createdAt: string
  updatedAt: string
}

export interface BriefingMedia {
  id: string
  sectionId: string
  fileName: string
  originalName: string
  mimeType: string
  size: number
  url: string
  createdAt: string
}

export interface Item {
  id: string
  sectionId: string
  title: string
  description?: string
  type: 'checkbox' | 'upload' | 'text' | 'signature'
  isRequired: boolean
  order: number

  // Configurações específicas por tipo
  config?: {
    allowedFileTypes?: string[]
    maxFileSize?: number
    maxFiles?: number
    placeholder?: string
  }

  status?: ItemStatus
  createdAt: string
  updatedAt: string
}

export interface ItemStatus {
  id: string
  itemId: string
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
  id: string
  itemStatusId: string
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
  checklistId: string
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentJson?: string
  contentHtml?: string
}

export interface CreateItemRequest {
  sectionId: string
  title: string
  description?: string
  type: 'checkbox' | 'upload' | 'text' | 'signature'
  isRequired: boolean
  order: number
  config?: {
    allowedFileTypes?: string[]
    maxFileSize?: number
    maxFiles?: number
    placeholder?: string
  }
}

export interface UpdateItemStatusRequest {
  itemId: string
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

// Tipos para o cliente público (sem autenticação)
export interface PublicChecklistView {
  id: string
  title: string
  description?: string
  deadline?: string
  sections: PublicSectionView[]
  progress: {
    totalItems: number
    completedItems: number
    percentage: number
  }
}

export interface PublicSectionView {
  id: string
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentHtml?: string
  items?: PublicItemView[]
}

export interface PublicItemView {
  id: string
  title: string
  description?: string
  type: 'checkbox' | 'upload' | 'text' | 'signature'
  isRequired: boolean
  order: number
  isCompleted: boolean
  config?: {
    allowedFileTypes?: string[]
    maxFileSize?: number
    maxFiles?: number
    placeholder?: string
  }
}
