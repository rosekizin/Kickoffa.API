'use client'

import { useEditor, EditorContent } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Link from '@tiptap/extension-link'
import Placeholder from '@tiptap/extension-placeholder'
import { Button } from '@/components/ui/button'

interface BriefingEditorProps {
  initialContent?: string
  onSave: (content: { contentJson: string; contentHtml: string }) => void
  sectionId?: string
  placeholder?: string
}

export const BriefingEditor = ({ 
  initialContent, 
  onSave, 
  sectionId, 
  placeholder = 'Comece a escrever ou insira uma imagem...' 
}: BriefingEditorProps) => {
  const editor = useEditor({
    extensions: [
      StarterKit,
      Image.configure({
        HTMLAttributes: {
          class: 'max-w-full h-auto rounded-lg',
        },
      }),
      Link.configure({
        openOnClick: false,
        HTMLAttributes: {
          class: 'text-blue-600 hover:text-blue-800 underline',
        },
      }),
      Placeholder.configure({
        placeholder,
      })
    ],
    content: initialContent,
    onUpdate: ({ editor }) => {
      // Opcional: auto-save
      const json = editor.getJSON()
      const html = editor.getHTML()
      console.log('Content updated', { json, html })
    }
  })

  const handleSave = () => {
    if (!editor) return
    
    onSave({
      contentJson: JSON.stringify(editor.getJSON()),
      contentHtml: editor.getHTML()
    })
  }

  const addImage = () => {
    const url = window.prompt('URL da imagem:')
    if (url && editor) {
      editor.chain().focus().setImage({ src: url }).run()
    }
  }

  const addLink = () => {
    const url = window.prompt('URL do link:')
    if (url && editor) {
      editor.chain().focus().setLink({ href: url }).run()
    }
  }

  if (!editor) {
    return <div>Carregando editor...</div>
  }

  return (
    <div className="border rounded-lg overflow-hidden">
      {/* Toolbar */}
      <div className="border-b bg-gray-50 p-2 flex gap-2 flex-wrap">
        <Button
          variant="outline"
          size="sm"
          onClick={() => editor.chain().focus().toggleBold().run()}
          className={editor.isActive('bold') ? 'bg-gray-200' : ''}
        >
          Negrito
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() => editor.chain().focus().toggleItalic().run()}
          className={editor.isActive('italic') ? 'bg-gray-200' : ''}
        >
          Itálico
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}
          className={editor.isActive('heading', { level: 2 }) ? 'bg-gray-200' : ''}
        >
          H2
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={() => editor.chain().focus().toggleBulletList().run()}
          className={editor.isActive('bulletList') ? 'bg-gray-200' : ''}
        >
          Lista
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={addImage}
        >
          Imagem
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={addLink}
        >
          Link
        </Button>
      </div>

      {/* Editor */}
      <div className="p-4">
        <EditorContent 
          editor={editor} 
          className="prose max-w-none min-h-[200px] focus:outline-none"
        />
      </div>

      {/* Actions */}
      <div className="border-t bg-gray-50 p-4 flex justify-end">
        <Button onClick={handleSave}>
          Salvar
        </Button>
      </div>
    </div>
  )
}
