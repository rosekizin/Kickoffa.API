'use client'

import { useState } from 'react'
import { Button } from '@/components/ui/button'
import {
  FileText,
  CheckSquare,
  GripVertical,
  Trash2,
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

export interface Section {
  id: string
  title: string
  type: 'briefing' | 'checklist'
  order: number
  contentJson?: string
  contentHtml?: string
  items?: any[]
}

interface SortableSectionProps {
  section: Section
  isActive: boolean
  onClick: () => void
  onDelete: () => void
}

function SortableSection({ section, isActive, onClick, onDelete }: SortableSectionProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: section.id })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={`group relative border rounded-lg p-3 cursor-pointer transition-all ${
        isActive
          ? 'border-blue-500 bg-blue-50'
          : 'border-gray-200 hover:border-gray-300 bg-white'
      } ${isDragging ? 'opacity-50 z-50' : ''}`}
      onClick={onClick}
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-3 flex-1">
          <div
            {...attributes}
            {...listeners}
            className="cursor-grab active:cursor-grabbing p-1 hover:bg-gray-100 rounded"
            title="Arrastar para reordenar"
          >
            <GripVertical className="h-4 w-4 text-gray-400" />
          </div>
          <div className="flex items-center space-x-2">
            {section.type === 'briefing' ? (
              <FileText className="h-4 w-4 text-blue-500" />
            ) : (
              <CheckSquare className="h-4 w-4 text-green-500" />
            )}
            <span className="text-sm font-medium text-gray-900">
              {section.title}
            </span>
          </div>
        </div>
        <Button
          variant="ghost"
          size="sm"
          onClick={(e) => {
            e.stopPropagation()
            onDelete()
          }}
          className="h-6 w-6 p-0 text-gray-400 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-opacity"
          title="Excluir seção"
        >
          <Trash2 className="h-3 w-3" />
        </Button>
      </div>

      {section.type === 'checklist' && section.items && (
        <div className="mt-2 text-xs text-gray-500">
          {section.items.length} item{section.items.length !== 1 ? 's' : ''}
        </div>
      )}
    </div>
  )
}

interface SortableSectionListProps {
  sections: Section[]
  activeSection: string | null
  onSectionClick: (sectionId: string) => void
  onSectionDelete: (sectionId: string) => void
  onSectionsReorder: (sections: Section[]) => void
}

export function SortableSectionList({
  sections,
  activeSection,
  onSectionClick,
  onSectionDelete,
  onSectionsReorder
}: SortableSectionListProps) {
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event

    if (over && active.id !== over.id) {
      const oldIndex = sections.findIndex((item) => item.id === active.id)
      const newIndex = sections.findIndex((item) => item.id === over.id)

      const newSections = arrayMove(sections, oldIndex, newIndex)
      
      // Atualizar a ordem das seções
      const reorderedSections = newSections.map((item, index) => ({
        ...item,
        order: index + 1
      }))

      onSectionsReorder(reorderedSections)
    }
  }

  if (sections.length === 0) {
    return (
      <div className="text-center py-8 text-gray-500">
        <FileText className="h-12 w-12 mx-auto mb-3 text-gray-300" />
        <p className="text-sm">Nenhuma seção criada ainda</p>
        <p className="text-xs text-gray-400 mt-1">
          Use os botões acima para adicionar seções
        </p>
      </div>
    )
  }

  return (
    <DndContext
      sensors={sensors}
      collisionDetection={closestCenter}
      onDragEnd={handleDragEnd}
    >
      <SortableContext 
        items={sections.map(s => s.id)} 
        strategy={verticalListSortingStrategy}
      >
        <div className="space-y-2">
          {sections.map((section) => (
            <SortableSection
              key={section.id}
              section={section}
              isActive={activeSection === section.id}
              onClick={() => onSectionClick(section.id)}
              onDelete={() => onSectionDelete(section.id)}
            />
          ))}
        </div>
      </SortableContext>
    </DndContext>
  )
}
