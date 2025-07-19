/**
 * Hook para interceptar e bloquear navegação quando há alterações não salvas
 *
 * Este hook implementa um sistema robusto de proteção contra perda de dados,
 * interceptando múltiplas formas de navegação no Next.js App Router:
 *
 * 1. beforeunload - Intercepta fechamento de aba/janela e recarregamento de página
 * 2. window.history - Intercepta navegação programática (pushState/replaceState)
 * 3. Cliques em links - Intercepta cliques em links internos da aplicação
 *
 * Quando uma tentativa de navegação é detectada e shouldBlock=true:
 * - A navegação é bloqueada
 * - onNavigationAttempt é chamado com o destino da navegação
 * - Um modal de confirmação pode ser exibido para o usuário
 *
 * Uso típico:
 * ```tsx
 * const { navigate, forceNavigate } = useNavigationGuard({
 *   shouldBlock: hasUnsavedChanges,
 *   onNavigationAttempt: (targetPath) => {
 *     setPendingNavigation(targetPath)
 *     setShowConfirmModal(true)
 *   }
 * })
 * ```
 */

'use client'

import { useEffect, useRef, useCallback } from 'react'
import { useRouter, usePathname } from 'next/navigation'

interface UseNavigationGuardOptions {
  shouldBlock: boolean
  onNavigationAttempt?: (targetPath: string) => void
  message?: string
}

export const useNavigationGuard = ({
  shouldBlock,
  onNavigationAttempt,
  message = 'Você tem alterações não salvas. Deseja realmente sair?'
}: UseNavigationGuardOptions) => {
  const router = useRouter()
  const pathname = usePathname()
  const currentPathRef = useRef(pathname)
  const isNavigatingRef = useRef(false) // Flag para evitar interceptar navegação própria

  // Atualizar referência do path atual quando a rota muda
  useEffect(() => {
    currentPathRef.current = pathname
  }, [pathname])

  // 1. INTERCEPTAR BEFOREUNLOAD
  // Bloqueia fechamento de aba/janela e recarregamento de página
  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (shouldBlock) {
        e.preventDefault()
        e.returnValue = message
        return message
      }
    }

    window.addEventListener('beforeunload', handleBeforeUnload)
    return () => window.removeEventListener('beforeunload', handleBeforeUnload)
  }, [shouldBlock, message])

  // 2. INTERCEPTAR NAVEGAÇÃO PROGRAMÁTICA
  // Sobrescreve window.history.pushState/replaceState para interceptar navegação do Next.js
  useEffect(() => {
    const originalPushState = window.history.pushState
    const originalReplaceState = window.history.replaceState

    // Interceptar pushState (usado pelo Next.js para navegação)
    window.history.pushState = function(data: any, unused: string, url?: string | URL | null) {
      if (shouldBlock && !isNavigatingRef.current && url && url !== currentPathRef.current) {
        console.log('🚫 History.pushState interceptado:', url)
        onNavigationAttempt?.(url.toString())
        return
      }
      return originalPushState.call(this, data, unused, url)
    }

    // Interceptar replaceState (menos comum, mas por segurança)
    window.history.replaceState = function(data: any, unused: string, url?: string | URL | null) {
      if (shouldBlock && !isNavigatingRef.current && url && url !== currentPathRef.current) {
        console.log('🚫 History.replaceState interceptado:', url)
        onNavigationAttempt?.(url.toString())
        return
      }
      return originalReplaceState.call(this, data, unused, url)
    }

    // Cleanup: restaurar métodos originais ao desmontar
    return () => {
      window.history.pushState = originalPushState
      window.history.replaceState = originalReplaceState
    }
  }, [shouldBlock, onNavigationAttempt])

  // 3. INTERCEPTAR CLIQUES EM LINKS
  // Intercepta cliques em elementos <a> para bloquear navegação via links
  useEffect(() => {
    const handleLinkClick = (e: MouseEvent) => {
      if (!shouldBlock) return

      const target = e.target as HTMLElement
      const link = target.closest('a[href]') as HTMLAnchorElement

      if (link && link.href) {
        const url = new URL(link.href)
        const targetPath = url.pathname

        // Verificar se é um link interno e diferente da página atual
        if (url.origin === window.location.origin && targetPath !== currentPathRef.current) {
          e.preventDefault() // Impedir navegação
          e.stopPropagation() // Impedir propagação do evento
          onNavigationAttempt?.(targetPath) // Notificar tentativa de navegação
        }
      }
    }

    // Usar capture=true para interceptar antes de outros handlers
    document.addEventListener('click', handleLinkClick, true)
    return () => document.removeEventListener('click', handleLinkClick, true)
  }, [shouldBlock, onNavigationAttempt])

  // FUNÇÕES PÚBLICAS DO HOOK

  /**
   * Navega para um path com verificação de bloqueio
   * Se shouldBlock=true, chama onNavigationAttempt ao invés de navegar
   */
  const navigate = useCallback((path: string) => {
    if (shouldBlock && path !== currentPathRef.current) {
      onNavigationAttempt?.(path)
    } else {
      isNavigatingRef.current = true // Marcar como navegação própria
      router.push(path)
      setTimeout(() => { isNavigatingRef.current = false }, 100)
    }
  }, [shouldBlock, onNavigationAttempt, router])

  /**
   * Força navegação ignorando qualquer bloqueio
   * Usado após confirmação do usuário ou salvamento bem-sucedido
   */
  const forceNavigate = useCallback((path: string) => {
    isNavigatingRef.current = true // Marcar como navegação própria
    router.push(path)
    setTimeout(() => { isNavigatingRef.current = false }, 100)
  }, [router])

  return {
    navigate,
    forceNavigate
  }
}
