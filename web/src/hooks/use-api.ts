import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '@/lib/api'
import { Team, Player, Match, CreateTeamRequest, CreatePlayerRequest, CreateMatchRequest } from '@/types'

// Teams
export const useTeams = () => {
  return useQuery({
    queryKey: ['teams'],
    queryFn: async () => {
      const response = await api.get<Team[]>('/teams')
      return response.data
    }
  })
}

export const useCreateTeam = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: async (data: CreateTeamRequest) => {
      const response = await api.post<Team>('/teams', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['teams'] })
    }
  })
}

// Players
export const usePlayers = (teamId?: string) => {
  return useQuery({
    queryKey: ['players', teamId],
    queryFn: async () => {
      const url = teamId ? `/players?teamId=${teamId}` : '/players'
      const response = await api.get<Player[]>(url)
      return response.data
    }
  })
}

export const useCreatePlayer = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: async (data: CreatePlayerRequest) => {
      const response = await api.post<Player>('/players', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['players'] })
    }
  })
}

// Matches
export const useMatches = () => {
  return useQuery({
    queryKey: ['matches'],
    queryFn: async () => {
      const response = await api.get<Match[]>('/matches')
      return response.data
    }
  })
}

export const useCreateMatch = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: async (data: CreateMatchRequest) => {
      const response = await api.post<Match>('/matches', data)
      return response.data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['matches'] })
    }
  })
}
