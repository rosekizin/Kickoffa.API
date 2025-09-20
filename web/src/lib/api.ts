import axios from 'axios'

// Tipos para o formato padrão de erro da API
export interface ApiValidationError {
  type: string
  title: string
  status: number
  errors: Record<string, string[]>
  traceId: string
}

export interface ApiErrorDetails {
  title: string
  message: string
  validationErrors: Record<string, string[]>
  status: number
  traceId?: string
}

// Serviço para processar erros da API
export const parseApiError = (error: any): ApiErrorDetails => {
  // Erro de resposta HTTP
  if (error.response?.data) {
    const apiError = error.response.data as ApiValidationError

    // Formato padrão da API com validação
    if (apiError.errors && typeof apiError.errors === 'object') {
      return {
        title: apiError.title || 'Erro de validação',
        message: apiError.title || 'Ocorreram erros de validação',
        validationErrors: apiError.errors,
        status: apiError.status || error.response.status,
        traceId: apiError.traceId
      }
    }

    // Outros tipos de erro da API
    return {
      title: apiError.title || 'Erro do servidor',
      message: apiError.title || error.response.statusText || 'Erro interno do servidor',
      validationErrors: {},
      status: error.response.status,
      traceId: apiError.traceId
    }
  }

  // Erro de rede
  if (error.request) {
    return {
      title: 'Erro de conexão',
      message: 'Não foi possível conectar ao servidor. Verifique sua conexão.',
      validationErrors: {},
      status: 0
    }
  }

  // Outros erros
  return {
    title: 'Erro inesperado',
    message: error.message || 'Ocorreu um erro inesperado',
    validationErrors: {},
    status: 0
  }
}

// Função para formatar mensagens de erro de validação
export const formatValidationErrors = (validationErrors: Record<string, string[]>): string => {
  const messages: string[] = []

  Object.entries(validationErrors).forEach(([, errors]) => {
    errors.forEach(error => {
      messages.push(error)
    })
  })

  return messages.join('\n')
}

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5084',
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 30000, // 30 segundos de timeout (aumentado para uploads)
  withCredentials: true // Sempre enviar cookies HttpOnly
})

// Interceptor de requisição para logging e garantir configurações
// Esse codigo intercepta todo e qualquer requisição antes de ser enviada
// Pode ser usada para logar dados ou modificar o request, etc
api.interceptors.request.use(
  (config) => {
    // Log da requisição para debug
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`)

    // Garantir que withCredentials está sempre true
    config.withCredentials = true

    // Log dos dados da requisição (sem dados sensíveis)
    if (config.data && config.method?.toLowerCase() === 'post') {
      const logData = { ...config.data }
      // Mascarar dados sensíveis
      if (logData.password) logData.password = '***'
      if (logData.email && config.url?.includes('/login')) {
        console.log(`📤 Request data: ${JSON.stringify({ email: logData.email, password: '***' })}`)
      } else {
        console.log(`📤 Request data: ${JSON.stringify(logData)}`)
      }
    }

    return config
  },
  (error) => {
    console.error('❌ Request Error:', error)
    return Promise.reject(error)
  }
)

// Interceptor de resposta para melhor tratamento de erros
// Esse codigo intercepta todo e qualquer response antes de ser recebido
// Pode ser usada para logar dados ou modificar o response, etc
api.interceptors.response.use(
  (response) => {
    // Log de sucesso para debug
    console.log(`✅ API Success: ${response.config.method?.toUpperCase()} ${response.config.url} - Status: ${response.status}`)
    return response
  },
  (error) => {
    // Processar erro usando o serviço
    const errorDetails = parseApiError(error)

    // Log detalhado do erro para debug
    console.error(`❌ API Error: ${error.config?.method?.toUpperCase()} ${error.config?.url}`)
    console.error(`📊 Status: ${errorDetails.status}`)
    console.error(`📋 Title: ${errorDetails.title}`)
    console.error(`📝 Message: ${errorDetails.message}`)

    if (Object.keys(errorDetails.validationErrors).length > 0) {
      console.error(`🔍 Validation Errors:`, errorDetails.validationErrors)
    }

    if (errorDetails.traceId) {
      console.error(`🔗 Trace ID: ${errorDetails.traceId}`)
    }



    // Anexar detalhes processados ao erro para uso posterior
    error.apiErrorDetails = errorDetails

    // Preservar o erro original para tratamento específico
    return Promise.reject(error)
  }
)

export default api
