'use client'

import { BriefingEditor } from '@/components/briefing/briefing-editor'
import { Button } from '@/components/ui/button'
import { CheckCircle, Upload, FileText, Zap, Shield, Smartphone } from 'lucide-react'

export default function Home() {
  const handleSave = (content: { contentJson: string; contentHtml: string }) => {
    console.log('Conteúdo salvo:', content)
    // Aqui você faria a chamada para a API
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              <div className="flex items-center space-x-2">
                <div className="w-8 h-8 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-lg flex items-center justify-center">
                  <Zap className="h-5 w-5 text-white" />
                </div>
                <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                  Kickoffa
                </h1>
              </div>
            </div>
            <nav className="flex space-x-4">
              <Button variant="ghost">Dashboard</Button>
              <Button variant="ghost">Checklists</Button>
              <Button variant="ghost">Clientes</Button>
              <Button>Criar Checklist</Button>
            </nav>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-gray-900 mb-4">
            Portal de Onboarding para Freelancers
          </h2>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Crie checklists estruturados com briefings ricos e colete tudo que você precisa
            dos seus clientes antes de começar qualquer projeto. Zero fricção, máxima eficiência.
          </p>
        </div>

        {/* Features Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 mb-12">
          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mb-4">
              <FileText className="h-6 w-6 text-blue-600" />
            </div>
            <h3 className="text-lg font-semibold mb-2">Briefing Estruturado</h3>
            <p className="text-gray-600">
              Editor rich text com TipTap para criar briefings detalhados com objetivos,
              público-alvo, prazos e orçamento.
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mb-4">
              <CheckCircle className="h-6 w-6 text-green-600" />
            </div>
            <h3 className="text-lg font-semibold mb-2">Checklist Interativo</h3>
            <p className="text-gray-600">
              Itens configuráveis com uploads de arquivos, checkboxes, campos de texto
              e assinaturas digitais.
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center mb-4">
              <Upload className="h-6 w-6 text-purple-600" />
            </div>
            <h3 className="text-lg font-semibold mb-2">Upload Drag & Drop</h3>
            <p className="text-gray-600">
              Interface moderna com drag-and-drop para logos, contratos,
              comprovantes e outros arquivos.
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <div className="w-12 h-12 bg-yellow-100 rounded-lg flex items-center justify-center mb-4">
              <Zap className="h-6 w-6 text-yellow-600" />
            </div>
            <h3 className="text-lg font-semibold mb-2">Notificações Automáticas</h3>
            <p className="text-gray-600">
              Lembretes via WhatsApp/e-mail para clientes e alertas instantâneos
              para freelancers.
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <div className="w-12 h-12 bg-indigo-100 rounded-lg flex items-center justify-center mb-4">
              <Shield className="h-6 w-6 text-indigo-600" />
            </div>
            <h3 className="text-lg font-semibold mb-2">Prova de Escopo</h3>
            <p className="text-gray-600">
              PDF de evidência carimbado com hash, data/hora e status.
              ZIP com todos os uploads para proteção legal.
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
            <div className="w-12 h-12 bg-pink-100 rounded-lg flex items-center justify-center mb-4">
              <Smartphone className="h-6 w-6 text-pink-600" />
            </div>
            <h3 className="text-lg font-semibold mb-2">Mobile-First</h3>
            <p className="text-gray-600">
              Interface responsiva otimizada para mobile. Clientes não precisam
              criar conta, apenas acessar o link único.
            </p>
          </div>
        </div>
      </section>

      {/* Demo do Editor */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="bg-white rounded-xl shadow-lg p-8">
          <div className="mb-6">
            <h3 className="text-2xl font-bold text-gray-900 mb-2">
              Editor de Briefing - TipTap UI
            </h3>
            <p className="text-gray-600">
              Crie briefings ricos com formatação profissional, imagens e links.
              Interface moderna similar ao editor do TipTap.dev.
            </p>
          </div>

          <BriefingEditor
            onSave={handleSave}
            placeholder="Descreva o objetivo do projeto, público-alvo, prazos, orçamento e qualquer informação relevante para o cliente..."
          />
        </div>
      </section>

      {/* CTA Section */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-xl p-8 text-center text-white">
          <h3 className="text-2xl font-bold mb-4">
            Pronto para revolucionar seu onboarding?
          </h3>
          <p className="text-blue-100 mb-6 max-w-2xl mx-auto">
            Comece gratuitamente com 1 checklist e 50MB de storage.
            Upgrade para Pro quando precisar de mais recursos.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button size="lg" variant="secondary">
              Começar Grátis
            </Button>
            <Button size="lg" variant="outline" className="border-white text-white hover:bg-white hover:text-blue-600">
              Ver Planos
            </Button>
          </div>
        </div>
      </section>
    </div>
  )
}
