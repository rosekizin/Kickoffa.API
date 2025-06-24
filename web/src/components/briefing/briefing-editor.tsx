'use client'

import { useEditor, EditorContent } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Link from '@tiptap/extension-link'
import Placeholder from '@tiptap/extension-placeholder'
import { Button } from '@/components/ui/button'
import {
  Bold,
  Italic,
  Strikethrough,
  Code,
  Link as LinkIcon,
  Image as ImageIcon,
  List,
  ListOrdered,
  Quote,
  Undo,
  Redo,
  Plus
} from 'lucide-react'

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
    immediatelyRender: false, // Fix para SSR
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
    const input = document.createElement('input')
    input.type = 'file'
    input.accept = 'image/*'
    input.onchange = async (e) => {
      const file = (e.target as HTMLInputElement).files?.[0]
      if (file && editor) {
        // TODO: Implementar upload real para o backend
        const url = URL.createObjectURL(file)
        editor.chain().focus().setImage({ src: url }).run()
      }
    }
    input.click()
  }

  const addLink = () => {
    const url = window.prompt('URL do link:')
    if (url && editor) {
      editor.chain().focus().setLink({ href: url }).run()
    }
  }

  if (!editor) {
    return (
      <div className="border rounded-lg bg-gray-50 p-8 text-center">
        <div className="animate-pulse">Carregando editor...</div>
      </div>
    )
  }

  return (
    <div className="border rounded-lg overflow-hidden bg-white shadow-sm">
      {/* Toolbar moderna similar ao TipTap.dev */}
      <div className="bg-gray-900 text-white p-3 flex items-center gap-1 flex-wrap">
        {/* Undo/Redo */}
        <div className="flex items-center gap-1 mr-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().undo().run()}
            disabled={!editor.can().undo()}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700"
          >
            <Undo className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().redo().run()}
            disabled={!editor.can().redo()}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700"
          >
            <Redo className="h-4 w-4" />
          </Button>
        </div>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Headings */}
        <div className="flex items-center gap-1 mr-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleHeading({ level: 1 }).run()}
            className={`h-8 px-2 text-sm font-semibold ${
              editor.isActive('heading', { level: 1 })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            H1
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleHeading({ level: 2 }).run()}
            className={`h-8 px-2 text-sm font-semibold ${
              editor.isActive('heading', { level: 2 })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            H2
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleHeading({ level: 3 }).run()}
            className={`h-8 px-2 text-sm font-semibold ${
              editor.isActive('heading', { level: 3 })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            H3
          </Button>
        </div>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Text formatting */}
        <div className="flex items-center gap-1 mr-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleBold().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('bold')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Bold className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleItalic().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('italic')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Italic className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleStrike().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('strike')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Strikethrough className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleCode().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('code')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Code className="h-4 w-4" />
          </Button>
        </div>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Lists */}
        <div className="flex items-center gap-1 mr-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleBulletList().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('bulletList')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <List className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleOrderedList().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('orderedList')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <ListOrdered className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().toggleBlockquote().run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive('blockquote')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Quote className="h-4 w-4" />
          </Button>
        </div>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Media */}
        <div className="flex items-center gap-1 mr-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={addLink}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700"
          >
            <LinkIcon className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={addImage}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700"
          >
            <ImageIcon className="h-4 w-4" />
          </Button>
        </div>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Add button */}
        <Button
          variant="ghost"
          size="sm"
          className="h-8 px-3 text-gray-300 hover:text-white hover:bg-gray-700 ml-auto"
        >
          <Plus className="h-4 w-4 mr-1" />
          Add
        </Button>
      </div>

      {/* Editor Content */}
      <div className="min-h-[400px] p-6 bg-white">
        <EditorContent
          editor={editor}
          className="prose prose-lg max-w-none min-h-[350px] focus:outline-none [&_.ProseMirror]:outline-none [&_.ProseMirror]:min-h-[350px]"
        />
      </div>

      {/* Save Actions */}
      <div className="border-t bg-gray-50 p-4 flex justify-between items-center">
        <div className="text-sm text-gray-500">
          Pressione Ctrl+S para salvar rapidamente
        </div>
        <Button onClick={handleSave} className="bg-blue-600 hover:bg-blue-700">
          Salvar Briefing
        </Button>
      </div>
    </div>
  )
}
