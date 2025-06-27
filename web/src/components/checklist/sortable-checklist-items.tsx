'use client'

import { Button } from '@/components/ui/button'
import {
  GripVertical,
  Trash2,
  Edit3,
  CheckSquare,
  Upload,
  Type,
  PenTool
} from 'lucide-react'
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from '@dnd-kit/core'
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable'
import {
  useSortable,
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'

// Usar o tipo Item existente, mas criar um alias para clareza
export type ChecklistItem = {
  id: string
  title: string
  type: 'checkbox' | 'upload' | 'text' | 'signature'
  description?: string
  isRequired?: boolean
  order: number
  config?: any
}

interface SortableChecklistItemProps {
  item: ChecklistItem
  onEdit: () => void
  onDelete: () => void
}

function SortableChecklistItem({ item, onEdit, onDelete }: SortableChecklistItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: item.id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  const getItemIcon = (type: string) => {
    switch (type) {
      case 'checkbox':
        return <CheckSquare className="h-4 w-4 text-green-600" />
      case 'upload':
        return <Upload className="h-4 w-4 text-blue-600" />
      case 'text':
        return <Type className="h-4 w-4 text-purple-600" />
      case 'signature':
        return <PenTool className="h-4 w-4 text-orange-600" />
      default:
        return <CheckSquare className="h-4 w-4 text-gray-600" />
    }
  }

  const getItemTypeName = (type: string) => {
    switch (type) {
      case 'checkbox':
        return 'caixa de seleção'
      case 'upload':
        return 'upload'
      case 'text':
        return 'texto'
      case 'signature':
        return 'assinatura'
      default:
        return type
    }
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={`group bg-white rounded-lg p-4 transition-all ${
        item.isRequired
          ? 'border-l-4 border-l-red-400 border-t border-r border-b border-gray-200'
          : 'border border-gray-200'
      } ${isDragging ? 'opacity-50 z-50 shadow-lg' : ''}`}
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-3 flex-1">
          <div
            {...attributes}
            {...listeners}
            className="cursor-grab active:cursor-grabbing p-1 hover:bg-gray-100 rounded opacity-0 group-hover:opacity-100 transition-opacity"
            title="Arrastar para reordenar"
          >
            <GripVertical className="h-4 w-4 text-gray-400" />
          </div>
          
          <div className="flex items-center space-x-2">
            {getItemIcon(item.type)}
            <div className="flex-1">
              <div className="flex items-center space-x-2">
                <h4 className="font-medium text-gray-900 flex items-center">
                  {item.title}
                  {item.isRequired && (
                    <span className="text-red-500 ml-1">*</span>
                  )}
                </h4>
                {item.isRequired ? (
                  <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-normal bg-red-50 text-red-600 border border-red-200">
                    Obrigatório
                  </span>
                ) : (
                  <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-normal bg-gray-50 text-gray-500 border border-gray-200">
                    Opcional
                  </span>
                )}
              </div>
              <p className="text-sm text-gray-500 mt-1">{getItemTypeName(item.type)}</p>
              {item.description && (
                <p className="text-xs text-gray-400 mt-1">{item.description}</p>
              )}
            </div>
          </div>
        </div>
        
        <div className="flex items-center space-x-2 opacity-0 group-hover:opacity-100 transition-opacity">
          <Button 
            variant="ghost" 
            size="sm"
            onClick={onEdit}
            className="text-blue-600 hover:text-blue-700"
            title="Editar item"
          >
            <Edit3 className="h-4 w-4" />
          </Button>
          <Button 
            variant="ghost" 
            size="sm"
            onClick={onDelete}
            className="text-red-600 hover:text-red-700"
            title="Excluir item"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  )
}

interface SortableChecklistItemsProps {
  items: ChecklistItem[]
  onItemsReorder: (items: ChecklistItem[]) => void
  onEditItem: (itemId: string) => void
  onDeleteItem: (itemId: string) => void
}

export function SortableChecklistItems({
  items,
  onItemsReorder,
  onEditItem,
  onDeleteItem
}: SortableChecklistItemsProps) {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event

    if (over && active.id !== over.id) {
      const oldIndex = items.findIndex((item) => item.id === active.id)
      const newIndex = items.findIndex((item) => item.id === over.id)

      const newItems = arrayMove(items, oldIndex, newIndex)
      
      // Atualizar a ordem dos itens
      const reorderedItems = newItems.map((item, index) => ({
        ...item,
        order: index + 1
      }))

      onItemsReorder(reorderedItems)
    }
  }

  if (items.length === 0) {
    return null
  }

  return (
    <div className="w-full">
      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
      >
        <SortableContext
          items={items.map(item => item.id)}
          strategy={verticalListSortingStrategy}
        >
          <div className="space-y-3 w-full">
            {items.map((item) => (
              <SortableChecklistItem
                key={item.id}
                item={item}
                onEdit={() => onEditItem(item.id)}
                onDelete={() => onDeleteItem(item.id)}
              />
            ))}
          </div>
        </SortableContext>
      </DndContext>
    </div>
  )
}
