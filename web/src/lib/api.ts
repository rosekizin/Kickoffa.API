import axios from 'axios'

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5084',
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000, // 10 segundos de timeout
  withCredentials: true // Sempre enviar cookies HttpOnly
})

// Interceptor de resposta para melhor tratamento de erros
api.interceptors.response.use(
  (response) => {
    // Log de sucesso para debug
    console.log(`✅ API Success: ${response.config.method?.toUpperCase()} ${response.config.url} - Status: ${response.status}`)
    return response
  },
  (error) => {
    // Log detalhado do erro para debug
    if (error.response) {
      console.error(`❌ API Error: ${error.config?.method?.toUpperCase()} ${error.config?.url}`)
      console.error(`📊 Status: ${error.response.status}`)
      console.error(`📋 Data:`, error.response.data)
      console.error(`📝 Headers:`, error.response.headers)
    } else if (error.request) {
      console.error('❌ Network Error: Sem resposta do servidor')
      console.error('📡 Request:', error.request)
    } else {
      console.error('❌ Request Setup Error:', error.message)
    }

    // Preservar o erro original para tratamento específico
    return Promise.reject(error)
  }
)

export default api
