import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '@/lib/api'
import {
  Checklist,
  Section,
  Item,
  CreateChecklistRequest,
  CreateSectionRequest,
  CreateItemRequest,
  UpdateItemStatusRequest,
  PublicChecklistView,
  Customer,
  CreateCustomerRequest
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
    mutationFn: async (data: CreateSectionRequest) => {
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

// Items
export const useCreateItem = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateItemRequest) => {
      const response = await api.post<Item>('/items', data)
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

export const useUpdateItemStatus = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: UpdateItemStatusRequest) => {
      const response = await api.post('/public/item-status', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['public-checklist'] })
    }
  })
}

export const useUploadFile = () => {
  return useMutation({
    mutationFn: async ({ file, itemId }: { file: File; itemId: string }) => {
      const formData = new FormData()
      formData.append('file', file)
      formData.append('itemId', itemId)

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

export const useCustomers = () => {
  return useQuery({
    queryKey: ['customers'],
    queryFn: () => CustomerService.getCustomers()
  })
}

export const useCustomer = (id: string) => {
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
    mutationFn: ({ id, data }: { id: string; data: Partial<CreateCustomerRequest> }) =>
      CustomerService.updateCustomer(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] })
    }
  })
}

export const useDeleteCustomer = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => CustomerService.deleteCustomer(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customers'] })
    }
  })
}

// Authentication
export const useLogin = () => {
  return useMutation({
    mutationFn: (loginData: LoginRequest) => AuthService.login(loginData)
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
