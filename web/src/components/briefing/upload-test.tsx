'use client'

import { useState } from 'react'
import { BriefingEditor } from './briefing-editor'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

export const UploadTest = () => {
  const [content, setContent] = useState('')
  const [isEditing, setIsEditing] = useState(true)
  const [lastUploadedUrl, setLastUploadedUrl] = useState<string>('')

  const handleSave = (content: string) => {
    setContent(content)
    console.log('Conteúdo salvo:', content)

    // Extrair URLs de imagens do conteúdo para debug
    const imageUrls = content.match(/src="([^"]*s3[^"]*)"/g)
    if (imageUrls) {
      console.log('🖼️ URLs de imagens encontradas:', imageUrls)
      setLastUploadedUrl(imageUrls[imageUrls.length - 1].replace('src="', '').replace('"', ''))
    }
  }

  const toggleEdit = () => {
    setIsEditing(!isEditing)
  }

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Teste de Upload de Imagens - AWS S3</CardTitle>
          <CardDescription>
            Teste as funcionalidades de upload de imagens no editor TipTap:
          </CardDescription>
          <div className="space-y-2 text-sm text-gray-600">
            <p>• <strong>Botão de imagem:</strong> Clique no ícone de imagem na toolbar</p>
            <p>• <strong>Drag & Drop:</strong> Arraste uma imagem para o editor</p>
            <p>• <strong>Paste:</strong> Copie uma imagem (Ctrl+C) e cole no editor (Ctrl+V)</p>
            <p>• <strong>Indicadores visuais:</strong> Observe o loading, progress e possíveis erros</p>
          </div>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <Button onClick={toggleEdit} variant="outline">
                {isEditing ? 'Parar Edição' : 'Iniciar Edição'}
              </Button>
              <span className="text-sm text-gray-500">
                Status: {isEditing ? 'Editando' : 'Visualizando'}
              </span>
            </div>

            <BriefingEditor
              initialContent={content}
              onSave={handleSave}
              placeholder="Digite aqui ou insira imagens usando os métodos acima..."
              isEditing={isEditing}
              onEditingChange={setIsEditing}
            />
          </div>
        </CardContent>
      </Card>

      {/* Instruções detalhadas */}
      <Card>
        <CardHeader>
          <CardTitle>Instruções de Teste</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <h4 className="font-semibold text-sm mb-2">1. Teste via Botão</h4>
            <p className="text-sm text-gray-600">
              Clique no ícone de imagem na toolbar e selecione uma imagem do seu computador.
              Observe o ícone mudar para um spinner durante o upload.
            </p>
          </div>

          <div>
            <h4 className="font-semibold text-sm mb-2">2. Teste via Drag & Drop</h4>
            <p className="text-sm text-gray-600">
              Arraste uma imagem do seu explorador de arquivos diretamente para o editor.
              A imagem deve aparecer na posição onde você soltou.
            </p>
          </div>

          <div>
            <h4 className="font-semibold text-sm mb-2">3. Teste via Paste</h4>
            <p className="text-sm text-gray-600">
              Copie uma imagem (de outro site, aplicativo, etc.) e cole no editor usando Ctrl+V.
              A imagem deve ser inserida na posição do cursor.
            </p>
          </div>

          <div>
            <h4 className="font-semibold text-sm mb-2">4. Indicadores Visuais</h4>
            <ul className="text-sm text-gray-600 space-y-1">
              <li>• <strong>Durante upload:</strong> Imagem com borda tracejada azul e animação pulse</li>
              <li>• <strong>Progress:</strong> Indicador no canto superior esquerdo</li>
              <li>• <strong>Erro:</strong> Mensagem vermelha com botão para fechar</li>
              <li>• <strong>Sucesso:</strong> Imagem normal sem indicadores</li>
            </ul>
          </div>

          <div>
            <h4 className="font-semibold text-sm mb-2">5. Validações</h4>
            <ul className="text-sm text-gray-600 space-y-1">
              <li>• Apenas imagens são aceitas (JPEG, PNG, GIF, WebP, SVG)</li>
              <li>• Tamanho máximo: 10MB</li>
              <li>• Arquivos vazios são rejeitados</li>
            </ul>
          </div>

          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
            <h4 className="font-semibold text-sm mb-2 text-yellow-800">⚠️ Configuração Necessária</h4>
            <p className="text-sm text-yellow-700">
              Para que o upload funcione, certifique-se de que:
            </p>
            <ul className="text-sm text-yellow-700 mt-2 space-y-1">
              <li>• O backend está rodando na porta 5084</li>
              <li>• As configurações AWS S3 estão corretas no appsettings.Development.json</li>
              <li>• Você está autenticado na aplicação</li>
            </ul>
          </div>
        </CardContent>
      </Card>

      {/* Debug info */}
      {content && (
        <Card>
          <CardHeader>
            <CardTitle>Conteúdo Atual (Debug)</CardTitle>
          </CardHeader>
          <CardContent>
            <pre className="text-xs bg-gray-100 p-4 rounded overflow-auto max-h-40">
              {content}
            </pre>
          </CardContent>
        </Card>
      )}

      {/* Teste de URL da imagem */}
      {lastUploadedUrl && (
        <Card>
          <CardHeader>
            <CardTitle>Teste de URL da Última Imagem</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <p className="text-sm font-medium mb-2">URL:</p>
              <code className="text-xs bg-gray-100 p-2 rounded block break-all">
                {lastUploadedUrl}
              </code>
            </div>

            <div>
              <p className="text-sm font-medium mb-2">Teste de Carregamento:</p>
              <img
                src={lastUploadedUrl}
                alt="Teste de carregamento"
                className="max-w-xs border rounded"
                onLoad={() => console.log('✅ Imagem carregada com sucesso')}
                onError={(e) => {
                  console.error('❌ Erro ao carregar imagem:', e)
                  console.error('URL que falhou:', lastUploadedUrl)
                }}
              />
            </div>

            <div>
              <p className="text-sm font-medium mb-2">Teste de Acesso Direto:</p>
              <a
                href={lastUploadedUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="text-blue-600 hover:underline text-sm"
              >
                Abrir imagem em nova aba
              </a>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
