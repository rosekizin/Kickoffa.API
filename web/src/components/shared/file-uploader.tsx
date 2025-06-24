'use client'

import { useState, useCallback } from 'react'
import { useDropzone } from 'react-dropzone'
import { Button } from '@/components/ui/button'
import { 
  Upload, 
  X, 
  File, 
  Image as ImageIcon, 
  FileText, 
  Video,
  Music,
  Archive,
  AlertCircle,
  CheckCircle
} from 'lucide-react'

interface FileUploaderProps {
  onFilesSelected: (files: File[]) => void
  maxFiles?: number
  maxSize?: number // em bytes
  acceptedTypes?: string[]
  multiple?: boolean
  disabled?: boolean
  className?: string
}

interface UploadedFile {
  file: File
  id: string
  progress: number
  status: 'uploading' | 'completed' | 'error'
  preview?: string
}

export const FileUploader = ({
  onFilesSelected,
  maxFiles = 5,
  maxSize = 10 * 1024 * 1024, // 10MB default
  acceptedTypes = ['image/*', 'application/pdf', '.doc', '.docx'],
  multiple = true,
  disabled = false,
  className = ''
}: FileUploaderProps) => {
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([])
  const [errors, setErrors] = useState<string[]>([])

  const onDrop = useCallback((acceptedFiles: File[], rejectedFiles: any[]) => {
    setErrors([])
    
    // Validar arquivos rejeitados
    if (rejectedFiles.length > 0) {
      const newErrors = rejectedFiles.map(({ file, errors }) => {
        if (errors.some((e: any) => e.code === 'file-too-large')) {
          return `${file.name}: Arquivo muito grande (máximo ${formatFileSize(maxSize)})`
        }
        if (errors.some((e: any) => e.code === 'file-invalid-type')) {
          return `${file.name}: Tipo de arquivo não permitido`
        }
        return `${file.name}: Erro desconhecido`
      })
      setErrors(newErrors)
    }

    // Validar limite de arquivos
    if (uploadedFiles.length + acceptedFiles.length > maxFiles) {
      setErrors(prev => [...prev, `Máximo de ${maxFiles} arquivos permitidos`])
      return
    }

    // Processar arquivos aceitos
    const newFiles: UploadedFile[] = acceptedFiles.map(file => ({
      file,
      id: Math.random().toString(36).substr(2, 9),
      progress: 0,
      status: 'uploading' as const,
      preview: file.type.startsWith('image/') ? URL.createObjectURL(file) : undefined
    }))

    setUploadedFiles(prev => [...prev, ...newFiles])
    onFilesSelected(acceptedFiles)

    // Simular upload (substituir por upload real)
    newFiles.forEach(uploadedFile => {
      simulateUpload(uploadedFile.id)
    })
  }, [uploadedFiles, maxFiles, maxSize, onFilesSelected])

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: acceptedTypes.reduce((acc, type) => ({ ...acc, [type]: [] }), {}),
    maxSize,
    multiple,
    disabled
  })

  const simulateUpload = (fileId: string) => {
    const interval = setInterval(() => {
      setUploadedFiles(prev => prev.map(file => {
        if (file.id === fileId) {
          const newProgress = Math.min(file.progress + 10, 100)
          return {
            ...file,
            progress: newProgress,
            status: newProgress === 100 ? 'completed' : 'uploading'
          }
        }
        return file
      }))
    }, 200)

    setTimeout(() => {
      clearInterval(interval)
      setUploadedFiles(prev => prev.map(file => 
        file.id === fileId ? { ...file, status: 'completed', progress: 100 } : file
      ))
    }, 2000)
  }

  const removeFile = (fileId: string) => {
    setUploadedFiles(prev => {
      const fileToRemove = prev.find(f => f.id === fileId)
      if (fileToRemove?.preview) {
        URL.revokeObjectURL(fileToRemove.preview)
      }
      return prev.filter(f => f.id !== fileId)
    })
  }

  const getFileIcon = (file: File) => {
    if (file.type.startsWith('image/')) return <ImageIcon className="h-5 w-5" />
    if (file.type.startsWith('video/')) return <Video className="h-5 w-5" />
    if (file.type.startsWith('audio/')) return <Music className="h-5 w-5" />
    if (file.type === 'application/pdf') return <FileText className="h-5 w-5" />
    if (file.type.includes('zip') || file.type.includes('rar')) return <Archive className="h-5 w-5" />
    return <File className="h-5 w-5" />
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Drop Zone */}
      <div
        {...getRootProps()}
        className={`
          border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-colors
          ${isDragActive 
            ? 'border-blue-400 bg-blue-50' 
            : 'border-gray-300 hover:border-gray-400'
          }
          ${disabled ? 'opacity-50 cursor-not-allowed' : ''}
        `}
      >
        <input {...getInputProps()} />
        <Upload className="h-12 w-12 text-gray-400 mx-auto mb-4" />
        
        {isDragActive ? (
          <p className="text-blue-600 font-medium">Solte os arquivos aqui...</p>
        ) : (
          <div>
            <p className="text-gray-600 font-medium mb-2">
              Clique para selecionar ou arraste arquivos aqui
            </p>
            <p className="text-sm text-gray-500">
              Máximo {maxFiles} arquivo{maxFiles > 1 ? 's' : ''}, até {formatFileSize(maxSize)} cada
            </p>
            <p className="text-xs text-gray-400 mt-1">
              Tipos aceitos: {acceptedTypes.join(', ')}
            </p>
          </div>
        )}
      </div>

      {/* Errors */}
      {errors.length > 0 && (
        <div className="space-y-2">
          {errors.map((error, index) => (
            <div key={index} className="flex items-center text-red-600 text-sm">
              <AlertCircle className="h-4 w-4 mr-2" />
              {error}
            </div>
          ))}
        </div>
      )}

      {/* Uploaded Files */}
      {uploadedFiles.length > 0 && (
        <div className="space-y-3">
          <h4 className="text-sm font-medium text-gray-700">
            Arquivos ({uploadedFiles.length}/{maxFiles})
          </h4>
          
          <div className="space-y-2">
            {uploadedFiles.map((uploadedFile) => (
              <div key={uploadedFile.id} className="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg">
                {/* File Icon/Preview */}
                <div className="flex-shrink-0">
                  {uploadedFile.preview ? (
                    <img 
                      src={uploadedFile.preview} 
                      alt={uploadedFile.file.name}
                      className="h-10 w-10 object-cover rounded"
                    />
                  ) : (
                    <div className="h-10 w-10 bg-gray-100 rounded flex items-center justify-center text-gray-500">
                      {getFileIcon(uploadedFile.file)}
                    </div>
                  )}
                </div>

                {/* File Info */}
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-900 truncate">
                    {uploadedFile.file.name}
                  </p>
                  <p className="text-xs text-gray-500">
                    {formatFileSize(uploadedFile.file.size)}
                  </p>
                  
                  {/* Progress Bar */}
                  {uploadedFile.status === 'uploading' && (
                    <div className="mt-1">
                      <div className="bg-gray-200 rounded-full h-1.5">
                        <div 
                          className="bg-blue-600 h-1.5 rounded-full transition-all duration-300"
                          style={{ width: `${uploadedFile.progress}%` }}
                        />
                      </div>
                    </div>
                  )}
                </div>

                {/* Status */}
                <div className="flex-shrink-0 flex items-center space-x-2">
                  {uploadedFile.status === 'completed' && (
                    <CheckCircle className="h-5 w-5 text-green-500" />
                  )}
                  {uploadedFile.status === 'error' && (
                    <AlertCircle className="h-5 w-5 text-red-500" />
                  )}
                  
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => removeFile(uploadedFile.id)}
                    className="h-8 w-8 p-0 text-gray-400 hover:text-red-500"
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
