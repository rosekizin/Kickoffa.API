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

// Usar o tipo Component existente, mas criar um alias para clareza
export type ChecklistComponent = {
  id: number
  title: string
  type: 'checkbox' | 'upload' | 'text' | 'signature' | 'confirmation'
  description?: string
  isRequired?: boolean
  order: number
  allowedMimeTypes?: string
  maxSizeMB?: number
  placeholder?: string
  maxLength?: number
  confirmationText?: string
}

interface SortableChecklistComponentProps {
  component: ChecklistComponent
  onEdit: () => void
  onDelete: () => void
}

function SortableChecklistComponent({ component, onEdit, onDelete }: SortableChecklistComponentProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: component.id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  const getComponentIcon = (type: string) => {
    switch (type) {
      case 'checkbox':
        return <CheckSquare className="h-4 w-4 text-green-600" />
      case 'upload':
        return <Upload className="h-4 w-4 text-blue-600" />
      case 'text':
        return <Type className="h-4 w-4 text-purple-600" />
      case 'signature':
        return <PenTool className="h-4 w-4 text-orange-600" />
      case 'confirmation':
        return <CheckSquare className="h-4 w-4 text-indigo-600" />
      default:
        return <CheckSquare className="h-4 w-4 text-gray-600" />
    }
  }

  const getComponentTypeName = (type: string) => {
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
        component.isRequired
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
            {getComponentIcon(component.type)}
            <div className="flex-1">
              <div className="flex items-center space-x-2">
                <h4 className="font-medium text-gray-900 flex items-center">
                  {component.title}
                  {component.isRequired && (
                    <span className="text-red-500 ml-1">*</span>
                  )}
                </h4>
                {component.isRequired ? (
                  <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-normal bg-red-50 text-red-600 border border-red-200">
                    Obrigatório
                  </span>
                ) : (
                  <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-normal bg-gray-50 text-gray-500 border border-gray-200">
                    Opcional
                  </span>
                )}
              </div>
              <p className="text-sm text-gray-500 mt-1">{getComponentTypeName(component.type)}</p>
              {component.description && (
                <p className="text-xs text-gray-400 mt-1">{component.description}</p>
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
            title="Editar componente"
          >
            <Edit3 className="h-4 w-4" />
          </Button>
          <Button 
            variant="ghost" 
            size="sm"
            onClick={onDelete}
            className="text-red-600 hover:text-red-700"
            title="Excluir componente"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  )
}

interface SortableChecklistComponentsProps {
  components: ChecklistComponent[]
  onComponentsReorder: (components: ChecklistComponent[]) => void
  onEditComponent: (componentId: number) => void
  onDeleteComponent: (componentId: number) => void
}

export function SortableChecklistComponents({
  components,
  onComponentsReorder,
  onEditComponent,
  onDeleteComponent
}: SortableChecklistComponentsProps) {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event

    if (over && active.id !== over.id) {
      const oldIndex = components.findIndex((component) => component.id === active.id)
      const newIndex = components.findIndex((component) => component.id === over.id)

      const newComponents = arrayMove(components, oldIndex, newIndex)
      
      // Atualizar a ordem dos componentes
      const reorderedComponents = newComponents.map((component, index) => ({
        ...component,
        order: index + 1
      }))

      onComponentsReorder(reorderedComponents)
    }
  }

  if (components.length === 0) {
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
          items={components.map(component => component.id)}
          strategy={verticalListSortingStrategy}
        >
          <div className="space-y-3 w-full">
            {components.map((component) => (
              <SortableChecklistComponent
                key={component.id}
                component={component}
                onEdit={() => onEditComponent(component.id)}
                onDelete={() => onDeleteComponent(component.id)}
              />
            ))}
          </div>
        </SortableContext>
      </DndContext>
    </div>
  )
}
