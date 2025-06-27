'use client'

import { useRef, useEffect, useState } from 'react'
import { Button } from '@/components/ui/button'
import { RotateCcw, Download, Pen, Check } from 'lucide-react'

interface SignaturePadProps {
  onSignatureChange: (signature: string | null) => void
  width?: number
  height?: number
  penColor?: string
  backgroundColor?: string
  disabled?: boolean
  className?: string
}

export const SignaturePad = ({
  onSignatureChange,
  width = 400,
  height = 200,
  penColor = '#000000',
  backgroundColor = '#ffffff',
  disabled = false,
  className = ''
}: SignaturePadProps) => {
  const canvasRef = useRef<HTMLCanvasElement>(null)
  const [isDrawing, setIsDrawing] = useState(false)
  const [isEmpty, setIsEmpty] = useState(true)
  const [lastPoint, setLastPoint] = useState<{ x: number; y: number } | null>(null)

  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas) return

    const ctx = canvas.getContext('2d')
    if (!ctx) return

    // Configurar canvas
    ctx.lineCap = 'round'
    ctx.lineJoin = 'round'
    ctx.lineWidth = 2
    ctx.strokeStyle = penColor

    // Preencher fundo
    ctx.fillStyle = backgroundColor
    ctx.fillRect(0, 0, width, height)
  }, [width, height, penColor, backgroundColor])

  const getEventPos = (e: React.MouseEvent | React.TouchEvent) => {
    const canvas = canvasRef.current
    if (!canvas) return { x: 0, y: 0 }

    const rect = canvas.getBoundingClientRect()
    const scaleX = canvas.width / rect.width
    const scaleY = canvas.height / rect.height

    if ('touches' in e) {
      // Touch event
      const touch = e.touches[0] || e.changedTouches[0]
      return {
        x: (touch.clientX - rect.left) * scaleX,
        y: (touch.clientY - rect.top) * scaleY
      }
    } else {
      // Mouse event
      return {
        x: (e.clientX - rect.left) * scaleX,
        y: (e.clientY - rect.top) * scaleY
      }
    }
  }

  const startDrawing = (e: React.MouseEvent | React.TouchEvent) => {
    if (disabled) return
    
    e.preventDefault()
    setIsDrawing(true)
    const pos = getEventPos(e)
    setLastPoint(pos)
  }

  const draw = (e: React.MouseEvent | React.TouchEvent) => {
    if (!isDrawing || disabled) return
    
    e.preventDefault()
    const canvas = canvasRef.current
    const ctx = canvas?.getContext('2d')
    if (!canvas || !ctx || !lastPoint) return

    const currentPos = getEventPos(e)
    
    ctx.beginPath()
    ctx.moveTo(lastPoint.x, lastPoint.y)
    ctx.lineTo(currentPos.x, currentPos.y)
    ctx.stroke()
    
    setLastPoint(currentPos)
    setIsEmpty(false)
  }

  const stopDrawing = () => {
    if (!isDrawing) return

    setIsDrawing(false)
    setLastPoint(null)

    // Não salvar automaticamente - usuário deve clicar em "Finalizar Assinatura"
  }

  const finalizeSignature = () => {
    const canvas = canvasRef.current
    if (canvas && !isEmpty) {
      const dataURL = canvas.toDataURL('image/png')
      onSignatureChange(dataURL)
    }
  }

  const clearSignature = () => {
    const canvas = canvasRef.current
    const ctx = canvas?.getContext('2d')
    if (!canvas || !ctx) return

    ctx.fillStyle = backgroundColor
    ctx.fillRect(0, 0, width, height)
    setIsEmpty(true)
    onSignatureChange(null)
  }

  const downloadSignature = () => {
    const canvas = canvasRef.current
    if (!canvas || isEmpty) return

    const link = document.createElement('a')
    link.download = 'assinatura.png'
    link.href = canvas.toDataURL('image/png')
    link.click()
  }

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Canvas */}
      <div className="relative border-2 border-gray-300 rounded-lg overflow-hidden bg-white">
        <canvas
          ref={canvasRef}
          width={width}
          height={height}
          className={`block ${disabled ? 'cursor-not-allowed opacity-50' : 'cursor-crosshair'}`}
          onMouseDown={startDrawing}
          onMouseMove={draw}
          onMouseUp={stopDrawing}
          onMouseLeave={stopDrawing}
          onTouchStart={startDrawing}
          onTouchMove={draw}
          onTouchEnd={stopDrawing}
          style={{ touchAction: 'none' }}
        />
        
        {/* Placeholder quando vazio */}
        {isEmpty && (
          <div className="absolute inset-0 flex items-center justify-center pointer-events-none">
            <div className="text-center text-gray-400">
              <Pen className="h-8 w-8 mx-auto mb-2" />
              <p className="text-sm">Assine aqui</p>
            </div>
          </div>
        )}
      </div>

      {/* Controls */}
      <div className="flex justify-between items-center">
        <div className="text-sm text-gray-600">
          {isEmpty ? 'Nenhuma assinatura' : 'Assinatura capturada'}
        </div>
        
        <div className="flex space-x-2">
          <Button
            onClick={finalizeSignature}
            disabled={disabled || isEmpty}
            className="bg-green-600 hover:bg-green-700 text-white"
          >
            <Check className="h-4 w-4 mr-2" />
            Finalizar Assinatura
          </Button>

          <Button
            variant="outline"
            size="sm"
            onClick={clearSignature}
            disabled={disabled || isEmpty}
          >
            <RotateCcw className="h-4 w-4 mr-2" />
            Limpar
          </Button>

          <Button
            variant="outline"
            size="sm"
            onClick={downloadSignature}
            disabled={disabled || isEmpty}
          >
            <Download className="h-4 w-4 mr-2" />
            Baixar
          </Button>
        </div>
      </div>

      {/* Instructions */}
      <div className="text-xs text-gray-500 bg-gray-50 p-3 rounded-lg">
        <p className="font-medium mb-1">Instruções:</p>
        <ul className="space-y-1">
          <li>• Use o mouse ou toque na tela para assinar</li>
          <li>• Mantenha pressionado e arraste para desenhar</li>
          <li>• Você pode soltar e continuar assinando quantas vezes quiser</li>
          <li>• Use "Limpar" para recomeçar</li>
          <li>• Clique em "Finalizar Assinatura" quando estiver satisfeito</li>
        </ul>
      </div>
    </div>
  )
}
