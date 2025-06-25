'use client'

import { useEditor, EditorContent } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Placeholder from '@tiptap/extension-placeholder'
import TextAlign from '@tiptap/extension-text-align'
import { useCallback, useState, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import {
  Bold,
  Italic,
  Strikethrough,
  Code,
  Image as ImageIcon,
  List,
  ListOrdered,
  Quote,
  Undo,
  Redo,
  ChevronDown,
  Type,
  Terminal,
  AlignLeft,
  AlignCenter,
  AlignRight,
  AlignJustify,

} from 'lucide-react'

interface BriefingEditorProps {
  initialContent?: string
  onSave: (content: { contentJson: string; contentHtml: string }) => void
  sectionId?: string
  placeholder?: string
}

interface DropdownProps {
  trigger: React.ReactNode
  children: React.ReactNode
  isOpen: boolean
  onToggle: () => void
}

const Dropdown = ({ trigger, children, isOpen, onToggle }: DropdownProps) => {
  return (
    <div className="relative">
      <div onClick={onToggle} className="cursor-pointer">
        {trigger}
      </div>
      {isOpen && (
        <div className="absolute top-full left-0 mt-1 bg-gray-800 border border-gray-600 rounded-md shadow-lg z-50 min-w-[160px]">
          {children}
        </div>
      )}
    </div>
  )
}

interface TooltipProps {
  content: string
  children: React.ReactNode
}

const Tooltip = ({ content, children }: TooltipProps) => {
  const [isVisible, setIsVisible] = useState(false)

  return (
    <div
      className="relative inline-block"
      onMouseEnter={() => setIsVisible(true)}
      onMouseLeave={() => setIsVisible(false)}
    >
      {children}
      {isVisible && (
        <div className="absolute top-full left-1/2 transform -translate-x-1/2 mt-2 px-2 py-1 bg-black text-white text-xs rounded whitespace-nowrap z-[9999] pointer-events-none">
          {content}
          <div className="absolute bottom-full left-1/2 transform -translate-x-1/2 w-0 h-0 border-l-4 border-r-4 border-b-4 border-transparent border-b-black"></div>
        </div>
      )}
    </div>
  )
}

export const BriefingEditor = ({
  initialContent,
  onSave,
  sectionId,
  placeholder = 'Comece a escrever ou insira uma imagem...'
}: BriefingEditorProps) => {
  const [headingDropdownOpen, setHeadingDropdownOpen] = useState(false)
  const [listDropdownOpen, setListDropdownOpen] = useState(false)

  // Fechar dropdowns quando clicar fora
  useEffect(() => {
    const handleClickOutside = () => {
      setHeadingDropdownOpen(false)
      setListDropdownOpen(false)
    }

    if (headingDropdownOpen || listDropdownOpen) {
      document.addEventListener('click', handleClickOutside)
      return () => document.removeEventListener('click', handleClickOutside)
    }
  }, [headingDropdownOpen, listDropdownOpen])
  const editor = useEditor({
    immediatelyRender: false, // Fix para SSR
    extensions: [
      StarterKit.configure({
        // Configurar explicitamente todas as extensões do StarterKit
        bold: {},
        italic: {},
        strike: {},
        code: {},
        heading: {
          levels: [1, 2, 3, 4, 5, 6],
        },
        bulletList: {
          HTMLAttributes: {
            class: 'list-disc list-inside',
          },
        },
        orderedList: {
          HTMLAttributes: {
            class: 'list-decimal list-inside',
          },
        },
        listItem: {},
        blockquote: {
          HTMLAttributes: {
            class: 'border-l-4 border-gray-300 pl-4 italic',
          },
        },
        codeBlock: {
          HTMLAttributes: {
            class: 'bg-gray-100 rounded p-4 font-mono text-sm',
          },
        },
        horizontalRule: {},
        hardBreak: {},
        history: {},
      }),
      Image.configure({
        HTMLAttributes: {
          class: 'max-w-full h-auto rounded-lg',
        },
      }),
      TextAlign.configure({
        types: ['heading', 'paragraph'],
        alignments: ['left', 'center', 'right', 'justify'],
        defaultAlignment: 'left',
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
    },
    editorProps: {
      attributes: {
        class: 'min-h-[350px] p-4 focus:outline-none',
      },
    },
  })



  const handleSave = () => {
    if (!editor) return

    onSave({
      contentJson: JSON.stringify(editor.getJSON()),
      contentHtml: editor.getHTML()
    })
  }

  const addImage = useCallback(() => {
    if (!editor) return

    const input = document.createElement('input')
    input.type = 'file'
    input.accept = 'image/*'
    input.onchange = async (e) => {
      const file = (e.target as HTMLInputElement).files?.[0]
      if (file) {
        // TODO: Implementar upload real para o backend
        const url = URL.createObjectURL(file)
        editor.chain().focus().setImage({ src: url }).run()
      }
    }
    input.click()
  }, [editor])





  const toggleHeading = useCallback((level: 1 | 2 | 3 | 4) => {
    if (!editor) return
    editor.chain().focus().toggleHeading({ level }).run()
    setHeadingDropdownOpen(false)
  }, [editor])

  const setParagraph = useCallback(() => {
    if (!editor) return
    editor.chain().focus().setParagraph().run()
    setHeadingDropdownOpen(false)
  }, [editor])

  const toggleBold = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleBold().run()
  }, [editor])

  const toggleItalic = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleItalic().run()
  }, [editor])

  const toggleStrike = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleStrike().run()
  }, [editor])

  const toggleCode = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleCode().run()
  }, [editor])

  const toggleCodeBlock = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleCodeBlock().run()
  }, [editor])

  const toggleBulletList = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleBulletList().run()
    setListDropdownOpen(false)
  }, [editor])

  const toggleOrderedList = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleOrderedList().run()
    setListDropdownOpen(false)
  }, [editor])

  const toggleBlockquote = useCallback(() => {
    if (!editor) return
    editor.chain().focus().toggleBlockquote().run()
  }, [editor])



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
        <Tooltip content="Desfazer">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().undo().run()}
            disabled={!editor.can().undo()}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700 disabled:opacity-50"
          >
            <Undo className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Refazer">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().redo().run()}
            disabled={!editor.can().redo()}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700 disabled:opacity-50"
          >
            <Redo className="h-4 w-4" />
          </Button>
        </Tooltip>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Heading Dropdown */}
        <Dropdown
          trigger={
            <Tooltip content="Títulos">
              <Button
                variant="ghost"
                size="sm"
                className={`h-8 px-2 text-sm font-semibold flex items-center gap-1 ${
                  editor.isActive('heading') || editor.isActive('paragraph')
                    ? 'bg-blue-600 text-white'
                    : 'text-gray-300 hover:text-white hover:bg-gray-700'
                }`}
              >
                <Type className="h-4 w-4" />
                <ChevronDown className="h-3 w-3" />
              </Button>
            </Tooltip>
          }
          isOpen={headingDropdownOpen}
          onToggle={() => setHeadingDropdownOpen(!headingDropdownOpen)}
        >
          <div className="py-1">
            <button
              onClick={setParagraph}
              className={`w-full text-left px-3 py-2 text-sm hover:bg-gray-700 ${
                editor.isActive('paragraph') ? 'bg-blue-600' : ''
              }`}
            >
              Parágrafo
            </button>
            <button
              onClick={() => toggleHeading(1)}
              className={`w-full text-left px-3 py-2 text-lg font-bold hover:bg-gray-700 ${
                editor.isActive('heading', { level: 1 }) ? 'bg-blue-600' : ''
              }`}
            >
              Título 1
            </button>
            <button
              onClick={() => toggleHeading(2)}
              className={`w-full text-left px-3 py-2 text-base font-bold hover:bg-gray-700 ${
                editor.isActive('heading', { level: 2 }) ? 'bg-blue-600' : ''
              }`}
            >
              Título 2
            </button>
            <button
              onClick={() => toggleHeading(3)}
              className={`w-full text-left px-3 py-2 text-sm font-bold hover:bg-gray-700 ${
                editor.isActive('heading', { level: 3 }) ? 'bg-blue-600' : ''
              }`}
            >
              Título 3
            </button>
            <button
              onClick={() => toggleHeading(4)}
              className={`w-full text-left px-3 py-2 text-xs font-bold hover:bg-gray-700 ${
                editor.isActive('heading', { level: 4 }) ? 'bg-blue-600' : ''
              }`}
            >
              Título 4
            </button>
          </div>
        </Dropdown>

        {/* List Dropdown */}
        <Dropdown
          trigger={
            <Tooltip content="Listas">
              <Button
                variant="ghost"
                size="sm"
                className={`h-8 px-2 text-sm flex items-center gap-1 ${
                  editor.isActive('bulletList') || editor.isActive('orderedList')
                    ? 'bg-blue-600 text-white'
                    : 'text-gray-300 hover:text-white hover:bg-gray-700'
                }`}
              >
                <List className="h-4 w-4" />
                <ChevronDown className="h-3 w-3" />
              </Button>
            </Tooltip>
          }
          isOpen={listDropdownOpen}
          onToggle={() => setListDropdownOpen(!listDropdownOpen)}
        >
          <div className="py-1">
            <button
              onClick={toggleBulletList}
              className={`w-full text-left px-3 py-2 text-sm hover:bg-gray-700 flex items-center gap-2 ${
                editor.isActive('bulletList') ? 'bg-blue-600' : ''
              }`}
            >
              <List className="h-4 w-4" />
              Lista com marcadores
            </button>
            <button
              onClick={toggleOrderedList}
              className={`w-full text-left px-3 py-2 text-sm hover:bg-gray-700 flex items-center gap-2 ${
                editor.isActive('orderedList') ? 'bg-blue-600' : ''
              }`}
            >
              <ListOrdered className="h-4 w-4" />
              Lista numerada
            </button>
          </div>
        </Dropdown>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Blockquote */}
        <Tooltip content="Citação">
          <Button
            variant="ghost"
            size="sm"
            onClick={toggleBlockquote}
            className={`h-8 w-8 p-0 ${
              editor.isActive('blockquote')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Quote className="h-4 w-4" />
          </Button>
        </Tooltip>

        {/* Code Block */}
        <Tooltip content="Bloco de código">
          <Button
            variant="ghost"
            size="sm"
            onClick={toggleCodeBlock}
            className={`h-8 w-8 p-0 ${
              editor.isActive('codeBlock')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Terminal className="h-4 w-4" />
          </Button>
        </Tooltip>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Text formatting */}
        <Tooltip content="Negrito">
          <Button
            variant="ghost"
            size="sm"
            onClick={toggleBold}
            className={`h-8 w-8 p-0 ${
              editor.isActive('bold')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Bold className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Itálico">
          <Button
            variant="ghost"
            size="sm"
            onClick={toggleItalic}
            className={`h-8 w-8 p-0 ${
              editor.isActive('italic')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Italic className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Riscado">
          <Button
            variant="ghost"
            size="sm"
            onClick={toggleStrike}
            className={`h-8 w-8 p-0 ${
              editor.isActive('strike')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Strikethrough className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Código inline">
          <Button
            variant="ghost"
            size="sm"
            onClick={toggleCode}
            className={`h-8 w-8 p-0 ${
              editor.isActive('code')
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <Code className="h-4 w-4" />
          </Button>
        </Tooltip>

        <div className="w-px h-6 bg-gray-600 mx-2" />

        {/* Text Alignment */}
        <Tooltip content="Alinhar à esquerda">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().setTextAlign('left').run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive({ textAlign: 'left' })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <AlignLeft className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Centralizar">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().setTextAlign('center').run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive({ textAlign: 'center' })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <AlignCenter className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Alinhar à direita">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().setTextAlign('right').run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive({ textAlign: 'right' })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <AlignRight className="h-4 w-4" />
          </Button>
        </Tooltip>

        <Tooltip content="Justificar">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => editor.chain().focus().setTextAlign('justify').run()}
            className={`h-8 w-8 p-0 ${
              editor.isActive({ textAlign: 'justify' })
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:text-white hover:bg-gray-700'
            }`}
          >
            <AlignJustify className="h-4 w-4" />
          </Button>
        </Tooltip>



        {/* Image */}
        <Tooltip content="Inserir imagem">
          <Button
            variant="ghost"
            size="sm"
            onClick={addImage}
            className="h-8 w-8 p-0 text-gray-300 hover:text-white hover:bg-gray-700"
          >
            <ImageIcon className="h-4 w-4" />
          </Button>
        </Tooltip>
      </div>

      {/* Editor Content */}
      <div className="min-h-[400px] p-6 bg-white relative">
        <EditorContent
          editor={editor}
          className="min-h-[350px] focus:outline-none [&_.ProseMirror]:outline-none [&_.ProseMirror]:min-h-[350px]"
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
