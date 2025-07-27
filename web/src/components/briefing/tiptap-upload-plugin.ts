import { Plugin, PluginKey } from '@tiptap/pm/state'
import { EditorView } from '@tiptap/pm/view'
import { BriefingUploadService } from '@/services/briefingUpload.service'

export interface UploadPluginOptions {
  onUploadStart?: (file: File) => void
  onUploadProgress?: (progress: number) => void
  onUploadSuccess?: (url: string, file: File) => void
  onUploadError?: (error: string, file: File) => void
}

// Chave única para o plugin
export const uploadPluginKey = new PluginKey('imageUpload')

// Mapa para rastrear uploads em andamento
const uploadingImages = new Map<string, { file: File; placeholderSrc: string }>()

export const createImageUploadPlugin = (options: UploadPluginOptions = {}) => {
  return new Plugin({
    key: uploadPluginKey,
    props: {
      // Interceptar paste de imagens
      handlePaste(view: EditorView, event: ClipboardEvent) {
        const items = Array.from(event.clipboardData?.items || [])
        
        for (const item of items) {
          if (item.type.indexOf('image') === 0) {
            event.preventDefault()
            const file = item.getAsFile()
            if (file) {
              handleImageUpload(file, view, options)
            }
            return true
          }
        }
        return false
      },

      // Interceptar drop de imagens
      handleDrop(view: EditorView, event: DragEvent, slice, moved) {
        if (!moved && event.dataTransfer) {
          const files = Array.from(event.dataTransfer.files)
          const imageFiles = files.filter(file => file.type.indexOf('image') === 0)
          
          if (imageFiles.length > 0) {
            event.preventDefault()
            
            // Calcular posição do drop
            const coordinates = view.posAtCoords({
              left: event.clientX,
              top: event.clientY
            })
            
            if (coordinates) {
              imageFiles.forEach(file => {
                handleImageUpload(file, view, options, coordinates.pos)
              })
            } else {
              // Fallback para posição atual do cursor
              imageFiles.forEach(file => {
                handleImageUpload(file, view, options)
              })
            }
            return true
          }
        }
        return false
      }
    }
  })
}

async function handleImageUpload(
  file: File, 
  view: EditorView, 
  options: UploadPluginOptions,
  position?: number
) {
  try {
    // Validar arquivo
    BriefingUploadService.validateImageFile(file)

    // Notificar início do upload
    options.onUploadStart?.(file)

    // Criar placeholder temporário
    const placeholderSrc = BriefingUploadService.createPreviewUrl(file)
    const uploadId = `upload_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
    
    // Armazenar informações do upload
    uploadingImages.set(uploadId, { file, placeholderSrc })

    // Inserir imagem placeholder na posição especificada ou na posição atual
    const { tr } = view.state
    const insertPos = position !== undefined ? position : view.state.selection.from
    
    const imageNode = view.state.schema.nodes.image.create({
      src: placeholderSrc,
      'data-upload-id': uploadId,
      'data-uploading': 'true',
      alt: 'Fazendo upload...',
      title: 'Upload em andamento...'
    })
    
    tr.insert(insertPos, imageNode)
    view.dispatch(tr)

    // Fazer upload real
    const result = await BriefingUploadService.uploadBriefingImage(
      file,
      (progress) => {
        options.onUploadProgress?.(progress.percentage)
      }
    )

    // Upload bem-sucedido - substituir placeholder pela URL real
    console.log('✅ Upload automático concluído:', file.name, result.url)

    const doc = view.state.doc
    let found = false

    doc.descendants((node, pos) => {
      if (found) return false

      if (node.type.name === 'image' &&
          node.attrs['data-upload-id'] === uploadId) {

        const newTr = view.state.tr
        newTr.setNodeMarkup(pos, null, {
          ...node.attrs,
          src: result.url,
          'data-upload-id': null,
          'data-uploading': null,
          alt: file.name,
          title: file.name
        })
        view.dispatch(newTr)
        found = true
      }
    })

    if (!found) {
      console.warn('⚠️ Nó de imagem não encontrado para uploadId:', uploadId)
    }

    // Limpar recursos
    BriefingUploadService.revokePreviewUrl(placeholderSrc)
    uploadingImages.delete(uploadId)

    // Notificar sucesso
    options.onUploadSuccess?.(result.url, file)

  } catch (error: any) {
    console.error('Erro no upload de imagem:', error)

    // Remover placeholder em caso de erro
    const doc = view.state.doc
    doc.descendants((node, pos) => {
      if (node.type.name === 'image' && 
          node.attrs['data-uploading'] === 'true' &&
          node.attrs.src.startsWith('blob:')) {
        
        const newTr = view.state.tr
        newTr.delete(pos, pos + node.nodeSize)
        view.dispatch(newTr)
        
        // Limpar blob URL
        BriefingUploadService.revokePreviewUrl(node.attrs.src)
      }
    })

    // Notificar erro
    options.onUploadError?.(error.message || 'Erro no upload', file)
  }
}

// Função para inserir imagem manualmente (via botão)
export function insertImageFromFile(
  file: File, 
  view: EditorView, 
  options: UploadPluginOptions = {}
) {
  handleImageUpload(file, view, options)
}

// Função para verificar se há uploads em andamento
export function hasUploadsInProgress(): boolean {
  return uploadingImages.size > 0
}

// Função para cancelar todos os uploads em andamento
export function cancelAllUploads(view: EditorView) {
  const doc = view.state.doc
  const tr = view.state.tr
  let hasChanges = false

  doc.descendants((node, pos) => {
    if (node.type.name === 'image' && node.attrs['data-uploading'] === 'true') {
      tr.delete(pos, pos + node.nodeSize)
      hasChanges = true
      
      // Limpar blob URL
      if (node.attrs.src.startsWith('blob:')) {
        BriefingUploadService.revokePreviewUrl(node.attrs.src)
      }
    }
  })

  if (hasChanges) {
    view.dispatch(tr)
  }

  // Limpar mapa de uploads
  uploadingImages.clear()
}

// Extensão personalizada do TipTap Image para suportar atributos de upload
export const ImageWithUpload = {
  addAttributes() {
    return {
      'data-upload-id': {
        default: null,
        parseHTML: element => element.getAttribute('data-upload-id'),
        renderHTML: attributes => {
          if (!attributes['data-upload-id']) {
            return {}
          }
          return {
            'data-upload-id': attributes['data-upload-id']
          }
        }
      },
      'data-uploading': {
        default: null,
        parseHTML: element => element.getAttribute('data-uploading'),
        renderHTML: attributes => {
          if (!attributes['data-uploading']) {
            return {}
          }
          return {
            'data-uploading': attributes['data-uploading']
          }
        }
      }
    }
  }
}
