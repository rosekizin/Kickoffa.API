// Tipos básicos do domínio
export interface User {
  id: string
  name: string
  email: string
  role: 'admin' | 'user'
  createdAt: string
  updatedAt: string
}

export interface Team {
  id: string
  name: string
  description?: string
  logoUrl?: string
  createdAt: string
  updatedAt: string
}

export interface Player {
  id: string
  name: string
  position: string
  teamId: string
  team?: Team
  createdAt: string
  updatedAt: string
}

export interface Match {
  id: string
  homeTeamId: string
  awayTeamId: string
  homeTeam?: Team
  awayTeam?: Team
  scheduledDate: string
  status: 'scheduled' | 'in_progress' | 'finished' | 'cancelled'
  homeScore?: number
  awayScore?: number
  createdAt: string
  updatedAt: string
}

// Tipos para formulários
export interface CreateTeamRequest {
  name: string
  description?: string
  logoUrl?: string
}

export interface CreatePlayerRequest {
  name: string
  position: string
  teamId: string
}

export interface CreateMatchRequest {
  homeTeamId: string
  awayTeamId: string
  scheduledDate: string
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
