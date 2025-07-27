# Integração de Upload de Arquivos com AWS S3

## Visão Geral

Este documento descreve como integrar o sistema de upload de arquivos com AWS S3 para resolver o problema de imagens blob no TipTap.

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

### 1. Configurar Plugin de Upload no TipTap

```typescript
import { Node } from '@tiptap/core'
import { Plugin, PluginKey } from 'prosemirror-plugin'

const uploadImagePlugin = new Plugin({
  key: new PluginKey('uploadImage'),
  props: {
    handlePaste(view, event, slice) {
      const items = Array.from(event.clipboardData?.items || [])
      
      for (const item of items) {
        if (item.type.indexOf('image') === 0) {
          event.preventDefault()
          const file = item.getAsFile()
          if (file) {
            uploadImage(file, view)
          }
          return true
        }
      }
      return false
    },
    handleDrop(view, event, slice, moved) {
      if (!moved && event.dataTransfer) {
        const files = Array.from(event.dataTransfer.files)
        const imageFiles = files.filter(file => file.type.indexOf('image') === 0)
        
        if (imageFiles.length > 0) {
          event.preventDefault()
          imageFiles.forEach(file => uploadImage(file, view))
          return true
        }
      }
      return false
    }
  }
})

async function uploadImage(file: File, view: any) {
  try {
    // Mostrar placeholder enquanto faz upload
    const placeholderSrc = URL.createObjectURL(file)
    const { tr } = view.state
    const pos = view.state.selection.from
    
    // Inserir imagem temporária
    tr.replaceSelectionWith(
      view.state.schema.nodes.image.create({ src: placeholderSrc })
    )
    view.dispatch(tr)
    
    // Fazer upload para o backend
    const formData = new FormData()
    formData.append('file', file)
    
    const response = await fetch('/api/fileupload/image/briefing', {
      method: 'POST',
      body: formData,
      credentials: 'include' // Para cookies de autenticação
    })
    
    if (!response.ok) {
      throw new Error('Erro no upload')
    }
    
    const result = await response.json()
    
    // Substituir placeholder pela URL real
    const newTr = view.state.tr
    const doc = view.state.doc
    
    doc.descendants((node: any, pos: number) => {
      if (node.type.name === 'image' && node.attrs.src === placeholderSrc) {
        newTr.setNodeMarkup(pos, null, { ...node.attrs, src: result.url })
        return false
      }
    })
    
    view.dispatch(newTr)
    
    // Limpar blob URL
    URL.revokeObjectURL(placeholderSrc)
    
  } catch (error) {
    console.error('Erro no upload:', error)
    // Remover placeholder em caso de erro
    // ... implementar lógica de erro
  }
}
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
      "LocalStackUrl": "http://localhost:4566"
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
      "LocalStackUrl": ""
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
