'use client'

import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { useState } from 'react'

interface QueryProviderProps {
  children: React.ReactNode
}

export function QueryProvider({ children }: QueryProviderProps) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // Configurações padrão para queries
            staleTime: 60 * 1000, // 1 minuto
            gcTime: 10 * 60 * 1000, // 10 minutos (anteriormente cacheTime)
            retry: (failureCount, error: any) => {
              // Não fazer retry para erros 4xx (exceto 408)
              if (error?.response?.status >= 400 && error?.response?.status < 500 && error?.response?.status !== 408) {
                return false
              }
              // Fazer retry até 3 vezes para outros erros
              return failureCount < 3
            },
            refetchOnWindowFocus: false, // Não refetch ao focar na janela
          },
          mutations: {
            // Configurações padrão para mutations
            retry: (failureCount, error: any) => {
              // Não fazer retry para erros 4xx
              if (error?.response?.status >= 400 && error?.response?.status < 500) {
                return false
              }
              // Fazer retry até 2 vezes para outros erros
              return failureCount < 2
            },
          },
        },
      })
  )

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      {/* DevTools apenas em desenvolvimento */}
      {process.env.NODE_ENV === 'development' && (
        <ReactQueryDevtools initialIsOpen={false} />
      )}
    </QueryClientProvider>
  )
}
