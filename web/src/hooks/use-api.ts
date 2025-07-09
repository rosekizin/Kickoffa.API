import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '@/lib/api'
import {
  Checklist,
  Section,
  Component,
  CreateChecklistRequest,
  CreateSectionRequest,
  CreateComponentRequest,
  UpdateComponentStatusRequest,
  PublicChecklistView,
  Customer,
  CreateCustomerRequest,
  LoginRequest,
  FileTypesSearchResponse,
  FileTypeCategory
} from '@/types'

// Checklists (para freelancers autenticados)
export const useChecklists = () => {
  return useQuery({
    queryKey: ['checklists'],
    queryFn: async () => {
      const response = await api.get<Checklist[]>('/checklists')
      return response.data
    }
  })
}

export const useChecklist = (id: string) => {
  return useQuery({
    queryKey: ['checklist', id],
    queryFn: async () => {
      const response = await api.get<Checklist>(`/checklists/${id}`)
      return response.data
    },
    enabled: !!id
  })
}

export const useCreateChecklist = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateChecklistRequest) => {
      const response = await api.post<Checklist>('/checklists', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['checklists'] })
    }
  })
}

// Sections
export const useCreateSection = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateSectionRequest & { checklistId: number }) => {
      const response = await api.post<Section>('/sections', data)
      return response.data
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['checklist', variables.checklistId] })
    }
  })
}

export const useUpdateSection = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async ({ id, data }: { id: string; data: Partial<CreateSectionRequest> }) => {
      const response = await api.put<Section>(`/sections/${id}`, data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['checklists'] })
    }
  })
}

// Components
export const useCreateComponent = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateComponentRequest) => {
      const response = await api.post<Component>('/components', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['checklists'] })
    }
  })
}

// Upload de mídia para briefing
export const useUploadBriefingMedia = () => {
  return useMutation({
    mutationFn: async ({ file, sectionId }: { file: File; sectionId: string }) => {
      const formData = new FormData()
      formData.append('file', file)
      formData.append('sectionId', sectionId)

      const response = await api.post('/briefing-media/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return response.data
    }
  })
}

// APIs públicas (sem autenticação) para clientes
export const usePublicChecklist = (token: string) => {
  return useQuery({
    queryKey: ['public-checklist', token],
    queryFn: async () => {
      const response = await api.get<PublicChecklistView>(`/public/checklist/${token}`)
      return response.data
    },
    enabled: !!token
  })
}

export const useUpdateComponentStatus = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: UpdateComponentStatusRequest) => {
      const response = await api.post('/public/component-status', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['public-checklist'] })
    }
  })
}

export const useUploadFile = () => {
  return useMutation({
    mutationFn: async ({ file, componentId }: { file: File; componentId: string }) => {
      const formData = new FormData()
      formData.append('file', file)
      formData.append('componentId', componentId)

      const response = await api.post('/public/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      return response.data
    }
  })
}

// Customers - usando CustomerService para lógica de negócio
import { CustomerService } from '@/services/customer.service'
import { AuthService } from '@/services/auth.service'
import { UserService, UpdateUserProfileRequest } from '@/services/user.service'

export const useCustomers = () => {
  return useQuery({
    queryKey: ['customers'],
    queryFn: () => CustomerService.getCustomers()
  })
}

export const useCustomer = (id: number) => {
  return useQuery({
    queryKey: ['customer', id],
    queryFn: () => CustomerService.getCustomerById(id),
    enabled: !!id
  })
}

export const useCreateCustomer = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateCustomerRequest) => CustomerService.createCustomer(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] })
    }
  })
}

export const useUpdateCustomer = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: Partial<CreateCustomerRequest> }) =>
      CustomerService.updateCustomer(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] })
    }
  })
}

export const useDeleteCustomer = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => CustomerService.deleteCustomer(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] })
    }
  })
}

// Authentication
export const useLogin = () => {
  return useMutation({
    mutationFn: (loginData: LoginRequest) => AuthService.login(loginData),
    onSuccess: (data) => {
      // Disparar evento para atualizar contexto
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('auth-login', { detail: data }))
      }
    }
  })
}

// User Profile
export const useUserProfile = () => {
  return useQuery({
    queryKey: ['user-profile'],
    queryFn: () => UserService.getCurrentUserProfile()
  })
}

export const useUpdateUserProfile = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: UpdateUserProfileRequest) => UserService.updateProfile(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['user-profile'] })
    }
  })
}

export const useCheckEmailAvailability = () => {
  return useMutation({
    mutationFn: (email: string) => UserService.checkEmailAvailability(email)
  })
}

export const useLogout = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: () => AuthService.logout(),
    onSuccess: () => {
      // Limpar todas as queries do cache após logout
      queryClient.clear()
    }
  })
}

export const useValidateToken = () => {
  return useQuery({
    queryKey: ['auth', 'validate'],
    queryFn: () => AuthService.validateToken(),
    enabled: AuthService.isAuthenticated(), // Baseado em dados do usuário
    staleTime: 5 * 60 * 1000, // 5 minutos
    retry: false
  })
}

// FileTypes
export const useFileTypes = () => {
  return useQuery({
    queryKey: ['fileTypes'],
    queryFn: async () => {
      const response = await api.get<FileTypesSearchResponse>('/api/filetype')
      return response.data
    }
  })
}

export const useSearchFileTypes = (searchTerm?: string) => {
  return useQuery({
    queryKey: ['fileTypes', 'search', searchTerm],
    queryFn: async () => {
      const params = searchTerm ? { search: searchTerm } : {}
      const response = await api.get<FileTypesSearchResponse>('/api/filetype/search', { params })
      return response.data
    },
    enabled: true // Sempre habilitado, mesmo sem searchTerm
  })
}

export const useFileTypesByCategory = (category: FileTypeCategory) => {
  return useQuery({
    queryKey: ['fileTypes', 'category', category],
    queryFn: async () => {
      const response = await api.get<FileTypesSearchResponse>(`/api/filetype/category/${category}`)
      return response.data
    }
  })
}
