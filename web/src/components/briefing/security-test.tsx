'use client'

import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { FileUploadService } from '@/services/fileUpload.service'

interface SecurityTestResult {
  test: string
  success: boolean
  message: string
  details?: any
}

export const SecurityTest = () => {
  const [results, setResults] = useState<SecurityTestResult[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [uploadedImageUrl, setUploadedImageUrl] = useState<string>('')

  const runSecurityTests = async () => {
    setIsLoading(true)
    const testResults: SecurityTestResult[] = []

    try {
      // 1. Fazer upload de uma imagem de teste
      const canvas = document.createElement('canvas')
      canvas.width = 50
      canvas.height = 50
      const ctx = canvas.getContext('2d')
      if (ctx) {
        ctx.fillStyle = '#ff0000'
        ctx.fillRect(0, 0, 50, 50)
      }

      const blob = await new Promise<Blob>((resolve) => {
        canvas.toBlob((blob) => resolve(blob!), 'image/png')
      })

      const testFile = new File([blob], 'security-test.png', { type: 'image/png' })
      
      console.log('🚀 Fazendo upload de teste...')
      const uploadResult = await FileUploadService.uploadBriefingImage(testFile)
      setUploadedImageUrl(uploadResult.url)

      testResults.push({
        test: '1. Upload de imagem',
        success: true,
        message: 'Upload realizado com sucesso',
        details: { url: uploadResult.url }
      })

      // 2. Testar acesso autenticado (deve funcionar)
      console.log('🔐 Testando acesso autenticado...')
      const authenticatedResponse = await fetch(uploadResult.url, {
        method: 'HEAD',
        credentials: 'include'
      })

      testResults.push({
        test: '2. Acesso autenticado',
        success: authenticatedResponse.ok,
        message: authenticatedResponse.ok 
          ? 'Acesso autenticado funcionou ✅' 
          : `Falha no acesso autenticado: ${authenticatedResponse.status}`,
        details: { 
          status: authenticatedResponse.status,
          headers: Object.fromEntries(authenticatedResponse.headers.entries())
        }
      })

      // 3. Testar acesso sem autenticação (deve falhar)
      console.log('🚫 Testando acesso sem autenticação...')
      const unauthenticatedResponse = await fetch(uploadResult.url, {
        method: 'HEAD',
        credentials: 'omit' // Não enviar cookies
      })

      const shouldFail = !unauthenticatedResponse.ok || unauthenticatedResponse.status === 401
      testResults.push({
        test: '3. Acesso sem autenticação',
        success: shouldFail,
        message: shouldFail 
          ? 'Acesso negado corretamente ✅' 
          : `FALHA DE SEGURANÇA: Acesso permitido sem autenticação! Status: ${unauthenticatedResponse.status}`,
        details: { 
          status: unauthenticatedResponse.status,
          shouldFail: true,
          actuallyFailed: !unauthenticatedResponse.ok
        }
      })

      // 4. Testar se URL direta do S3 não funciona (se conseguirmos extrair)
      if (uploadResult.url.includes('/api/images/')) {
        testResults.push({
          test: '4. Verificação de URL do proxy',
          success: true,
          message: 'URL retornada é do proxy (segura) ✅',
          details: { 
            isProxyUrl: true,
            url: uploadResult.url
          }
        })
      } else {
        testResults.push({
          test: '4. Verificação de URL do proxy',
          success: false,
          message: 'FALHA: URL retornada não é do proxy!',
          details: { 
            isProxyUrl: false,
            url: uploadResult.url
          }
        })
      }

      // 5. Testar headers de segurança
      console.log('🛡️ Verificando headers de segurança...')
      const headersResponse = await fetch(uploadResult.url, {
        method: 'HEAD',
        credentials: 'include'
      })

      const securityHeaders = {
        'x-content-type-options': headersResponse.headers.get('x-content-type-options'),
        'x-frame-options': headersResponse.headers.get('x-frame-options'),
        'cache-control': headersResponse.headers.get('cache-control'),
        'x-robots-tag': headersResponse.headers.get('x-robots-tag')
      }

      const hasSecurityHeaders = securityHeaders['x-content-type-options'] === 'nosniff' &&
                                securityHeaders['x-frame-options'] &&
                                securityHeaders['cache-control']?.includes('private')

      testResults.push({
        test: '5. Headers de segurança',
        success: hasSecurityHeaders,
        message: hasSecurityHeaders 
          ? 'Headers de segurança configurados ✅' 
          : 'Alguns headers de segurança estão faltando',
        details: securityHeaders
      })

    } catch (error: any) {
      testResults.push({
        test: 'Erro no teste',
        success: false,
        message: `Erro durante os testes: ${error.message}`,
        details: { error: error.stack }
      })
    }

    setResults(testResults)
    setIsLoading(false)
  }

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Teste de Segurança - Imagens Privadas</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <Button 
              onClick={runSecurityTests} 
              disabled={isLoading}
              className="w-full"
            >
              {isLoading ? 'Executando Testes...' : 'Executar Testes de Segurança'}
            </Button>

            {results.length > 0 && (
              <div className="space-y-3">
                <h3 className="font-semibold">Resultados dos Testes:</h3>
                {results.map((result, index) => (
                  <div 
                    key={index}
                    className={`p-4 rounded border ${
                      result.success 
                        ? 'bg-green-50 border-green-200' 
                        : 'bg-red-50 border-red-200'
                    }`}
                  >
                    <div className="flex items-center gap-2 mb-2">
                      <span className={`text-sm font-medium ${
                        result.success ? 'text-green-800' : 'text-red-800'
                      }`}>
                        {result.success ? '✅' : '❌'} {result.test}
                      </span>
                    </div>
                    
                    <p className={`text-sm mb-2 ${
                      result.success ? 'text-green-700' : 'text-red-700'
                    }`}>
                      {result.message}
                    </p>

                    {result.details && (
                      <details className="text-xs">
                        <summary className="cursor-pointer text-gray-600 hover:text-gray-800">
                          Ver detalhes
                        </summary>
                        <pre className="mt-2 bg-white p-2 rounded border overflow-auto max-h-32">
                          {JSON.stringify(result.details, null, 2)}
                        </pre>
                      </details>
                    )}
                  </div>
                ))}
              </div>
            )}

            {uploadedImageUrl && (
              <div className="mt-6 p-4 bg-blue-50 border border-blue-200 rounded">
                <h4 className="font-semibold text-blue-800 mb-2">Teste Manual:</h4>
                <p className="text-sm text-blue-700 mb-2">
                  Copie a URL abaixo e tente acessá-la em uma aba anônima/privada:
                </p>
                <code className="text-xs bg-white p-2 rounded border block break-all">
                  {uploadedImageUrl}
                </code>
                <p className="text-xs text-blue-600 mt-2">
                  ✅ <strong>Esperado:</strong> Erro 401 (Unauthorized) ou redirecionamento para login<br/>
                  ❌ <strong>Falha de segurança:</strong> Se a imagem carregar normalmente
                </p>
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
