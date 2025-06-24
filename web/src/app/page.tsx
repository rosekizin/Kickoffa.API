'use client'

import { BriefingEditor } from '@/components/briefing/briefing-editor'
import { Button } from '@/components/ui/button'

export default function Home() {
  const handleSave = (content: { contentJson: string; contentHtml: string }) => {
    console.log('Conteúdo salvo:', content)
    // Aqui você faria a chamada para a API
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              <h1 className="text-2xl font-bold text-gray-900">Kickoffa</h1>
            </div>
            <nav className="flex space-x-4">
              <Button variant="ghost">Dashboard</Button>
              <Button variant="ghost">Times</Button>
              <Button variant="ghost">Jogadores</Button>
              <Button variant="ghost">Partidas</Button>
            </nav>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="mb-8">
            <h2 className="text-3xl font-bold text-gray-900 mb-2">
              Bem-vindo ao Kickoffa
            </h2>
            <p className="text-gray-600">
              Sistema de gestão para times e competições de futebol
            </p>
          </div>

          {/* Demo do Editor */}
          <div className="bg-white rounded-lg shadow p-6 mb-8">
            <h3 className="text-xl font-semibold mb-4">Editor de Briefing (Demo)</h3>
            <BriefingEditor
              onSave={handleSave}
              placeholder="Digite aqui o briefing da partida..."
            />
          </div>

          {/* Cards de funcionalidades */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div className="bg-white rounded-lg shadow p-6">
              <h3 className="text-lg font-semibold mb-2">Gestão de Times</h3>
              <p className="text-gray-600 mb-4">
                Gerencie informações dos times, jogadores e comissão técnica.
              </p>
              <Button>Ver Times</Button>
            </div>

            <div className="bg-white rounded-lg shadow p-6">
              <h3 className="text-lg font-semibold mb-2">Controle de Partidas</h3>
              <p className="text-gray-600 mb-4">
                Acompanhe partidas, resultados e estatísticas em tempo real.
              </p>
              <Button>Ver Partidas</Button>
            </div>

            <div className="bg-white rounded-lg shadow p-6">
              <h3 className="text-lg font-semibold mb-2">Relatórios</h3>
              <p className="text-gray-600 mb-4">
                Gere relatórios detalhados sobre performance e estatísticas.
              </p>
              <Button>Ver Relatórios</Button>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
