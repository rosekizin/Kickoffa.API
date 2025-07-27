# Integração de Upload de Briefing com AWS S3

## Visão Geral

Este documento descreve como integrar o sistema de upload de imagens de briefing com AWS S3 para resolver o problema de imagens blob no TipTap.

**Nota:** Esta implementação é específica para uploads de briefing. Para uploads de cliente, consulte a documentação de expansão futura.

## Problema Resolvido

Anteriormente, quando usuários inseriam imagens no TipTap (seções de briefing), as imagens eram armazenadas como blob URLs:
```json
{"type":"image","attrs":{"src":"blob:http://localhost:3000/20e9403f-d76a-4370-a037-180851ada3cc"}}
```

Essas URLs blob se perdiam quando:
- A API era reiniciada
- O usuário fazia logout
- O navegador era fechado

## Solução Implementada

### Backend

1. **Serviço de Upload (`IFileUploadService`)**
   - Upload de imagens para AWS S3
   - Validação de tipos de arquivo
   - Geração de nomes únicos
   - URLs pré-assinadas para acesso privado

2. **Controller (`FileUploadController`)**
   - Endpoint específico para imagens de briefing: `POST /api/fileupload/image/briefing`
   - Endpoint genérico para arquivos: `POST /api/fileupload/file`
   - Endpoint para remoção: `DELETE /api/fileupload`

3. **Configuração AWS S3**
   - Configurações no `appsettings.json`
   - Suporte a LocalStack para desenvolvimento
   - Arquivos privados por padrão

### Endpoints Disponíveis

#### Upload de Imagem para Briefing
```http
POST /api/fileupload/image/briefing
Content-Type: multipart/form-data
Authorization: Bearer {token}

file: [arquivo de imagem]
```

**Resposta:**
```json
{
  "url": "https://bucket.s3.region.amazonaws.com/briefing/images/filename_20240726-143022_a1b2c3d4.jpg",
  "fileName": "original-filename.jpg",
  "contentType": "image/jpeg",
  "size": 1024000
}
```

#### Upload de Arquivo Genérico
```http
POST /api/fileupload/file?folder=uploads
Content-Type: multipart/form-data
Authorization: Bearer {token}

file: [arquivo]
```

#### Remoção de Arquivo
```http
DELETE /api/fileupload?fileUrl=https://bucket.s3.region.amazonaws.com/path/file.jpg
Authorization: Bearer {token}
```

## Integração com Frontend (TipTap)

### 1. Usar Serviços de Briefing

```typescript
// Importar serviços específicos para briefing
import { BriefingUploadService } from '@/services/briefingUpload.service'
import { useBriefingUpload } from '@/hooks/use-briefing-upload'

// Hook para gerenciar upload
const { uploadState, uploadImage, clearError } = useBriefingUpload()

// Upload manual via botão
const handleImageUpload = async (file: File) => {
  try {
    const result = await uploadImage(file)
    if (result) {
      // Inserir no editor TipTap
      editor.chain().focus().setImage({
        src: result.url,
        alt: file.name
      }).run()
    }
  } catch (error) {
    console.error('Erro no upload:', error)
  }
}

// Upload automático via plugin (drag & drop, paste)
import { createImageUploadPlugin } from './tiptap-upload-plugin'

const uploadPlugin = createImageUploadPlugin({
  onUploadStart: (file) => console.log('Upload iniciado:', file.name),
  onUploadSuccess: (url, file) => console.log('Upload concluído:', url),
  onUploadError: (error, file) => console.error('Erro:', error)
})

editor.registerPlugin(uploadPlugin)
```

### 2. Configurar Editor TipTap

```typescript
import { useEditor } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'

const editor = useEditor({
  extensions: [
    StarterKit,
    Image.configure({
      HTMLAttributes: {
        class: 'tiptap-image',
      },
    }),
  ],
  content: initialContent,
  editorProps: {
    attributes: {
      class: 'prose prose-sm sm:prose lg:prose-lg xl:prose-2xl mx-auto focus:outline-none',
    },
  },
})

// Adicionar plugin de upload
if (editor) {
  editor.registerPlugin(uploadImagePlugin)
}
```

### 3. Botão de Upload Manual

```typescript
const handleImageUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
  const file = event.target.files?.[0]
  if (!file || !editor) return
  
  try {
    const formData = new FormData()
    formData.append('file', file)
    
    const response = await fetch('/api/fileupload/image/briefing', {
      method: 'POST',
      body: formData,
      credentials: 'include'
    })
    
    const result = await response.json()
    
    // Inserir imagem no editor
    editor.chain().focus().setImage({ src: result.url }).run()
    
  } catch (error) {
    console.error('Erro no upload:', error)
  }
}

// Componente
<input
  type="file"
  accept="image/*"
  onChange={handleImageUpload}
  style={{ display: 'none' }}
  ref={fileInputRef}
/>
<button onClick={() => fileInputRef.current?.click()}>
  Inserir Imagem
</button>
```

## Estrutura de Arquivos Frontend

```
web/src/
├── services/
│   └── briefingUpload.service.ts     # Serviço específico para briefing
├── hooks/
│   └── use-briefing-upload.ts        # Hook para gerenciar estado de upload
├── components/briefing/
│   ├── briefing-editor.tsx           # Editor TipTap com upload
│   └── tiptap-upload-plugin.ts       # Plugin para drag&drop e paste
```

## Nomenclatura e Organização

**Frontend (Específico por Contexto):**
- `BriefingUploadService` - Para uploads de briefing
- `useBriefingUpload` - Hook específico para briefing
- Futuramente: `ClientUploadService` para uploads de cliente

**Backend (Genérico e Extensível):**
- `FileUploadController` - Controller genérico para todos os uploads
- `ImageProxyController` - Proxy seguro para servir imagens
- Endpoints organizados por contexto: `/briefing`, `/client`, etc.

## Configuração AWS S3

### Desenvolvimento (appsettings.Development.json)
```json
{
  "AWS": {
    "S3": {
      "BucketName": "kickoffa-dev-uploads",
      "Region": "us-east-1",
      "AccessKey": "your-access-key",
      "SecretKey": "your-secret-key",
      "BaseUrl": "https://kickoffa-dev-uploads.s3.us-east-1.amazonaws.com",
      "UseLocalStack": false,
      "LocalStackUrl": "http://localhost:4566",
      "ApiBaseUrl": "http://localhost:5084"
    }
  }
}
```

### Produção (appsettings.json)
```json
{
  "AWS": {
    "S3": {
      "BucketName": "",
      "Region": "us-east-1",
      "AccessKey": "",
      "SecretKey": "",
      "BaseUrl": "",
      "UseLocalStack": false,
      "LocalStackUrl": "",
      "ApiBaseUrl": "https://api.kickoffa.com"
    }
  }
}
```

## Segurança

1. **Autenticação**: Todos os endpoints requerem autenticação
2. **Validação**: Tipos de arquivo e tamanhos são validados
3. **Arquivos Privados**: Por padrão, arquivos são privados no S3
4. **URLs Pré-assinadas**: Para acesso temporário a arquivos privados

## Estrutura de Pastas no S3

```
bucket-name/
├── briefing/
│   └── images/
│       ├── image1_20240726-143022_a1b2c3d4.jpg
│       └── image2_20240726-143023_b2c3d4e5.png
└── uploads/
    ├── document1_20240726-143024_c3d4e5f6.pdf
    └── file1_20240726-143025_d4e5f6g7.docx
```

## Próximos Passos

1. Configurar bucket S3 na AWS
2. Configurar credenciais de acesso
3. Implementar integração no frontend
4. Testar upload e visualização de imagens
5. Implementar limpeza de arquivos órfãos (opcional)
