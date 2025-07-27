import { Node, mergeAttributes } from '@tiptap/core'
import { ReactNodeViewRenderer, NodeViewWrapper } from '@tiptap/react'
import { UploadArea } from './upload-area'

// Componente React para o Node View
const UploadAreaNodeView = ({ editor, deleteNode, getPos }: any) => {
  const handleImageUploaded = (url: string, fileName: string) => {
    // Obter posição atual do nó
    const pos = getPos()

    if (pos !== undefined) {
      // Substituir o nó de upload pela imagem
      const tr = editor.state.tr
      tr.replaceWith(pos, pos + 1, editor.schema.nodes.image.create({
        src: url,
        alt: fileName,
        title: fileName
      }))
      editor.view.dispatch(tr)
    }
  }

  return (
    <NodeViewWrapper className="upload-area-wrapper">
      <div className="my-4">
        <UploadArea
          onImageUploaded={handleImageUploaded}
          className="max-w-md mx-auto"
        />
      </div>
    </NodeViewWrapper>
  )
}

// Extensão TipTap para área de upload
export const UploadAreaExtension = Node.create({
  name: 'uploadArea',
  
  group: 'block',
  
  atom: true,
  
  addAttributes() {
    return {
      id: {
        default: null,
      },
    }
  },

  parseHTML() {
    return [
      {
        tag: 'div[data-type="upload-area"]',
      },
    ]
  },

  renderHTML({ HTMLAttributes }) {
    return ['div', mergeAttributes(HTMLAttributes, { 'data-type': 'upload-area' })]
  },

  addNodeView() {
    return ReactNodeViewRenderer(UploadAreaNodeView)
  },

  addCommands() {
    return {
      insertUploadArea: () => ({ commands }) => {
        return commands.insertContent({
          type: this.name,
          attrs: {
            id: `upload-${Date.now()}`,
          },
        })
      },
    }
  },
})

// Hook para usar a extensão
export const useUploadAreaExtension = () => {
  return UploadAreaExtension
}
