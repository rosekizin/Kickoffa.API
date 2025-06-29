'use client'

import Link from 'next/link'
import { DashboardLayout } from '@/components/layout/dashboard-layout'
import { 
  User, 
  Bell, 
  Shield, 
  Palette, 
  HelpCircle,
  ChevronRight,
  Settings
} from 'lucide-react'

const settingsCategories = [
  {
    title: 'Conta',
    description: 'Gerencie suas informações pessoais e configurações de conta',
    items: [
      {
        name: 'Perfil',
        description: 'Atualize suas informações pessoais',
        href: '/settings/profile',
        icon: User,
        color: 'bg-blue-100 text-blue-600'
      },
      {
        name: 'Segurança',
        description: 'Configurações de senha e autenticação',
        href: '/settings/security',
        icon: Shield,
        color: 'bg-green-100 text-green-600'
      }
    ]
  },
  {
    title: 'Preferências',
    description: 'Personalize sua experiência na plataforma',
    items: [
      {
        name: 'Notificações',
        description: 'Configure como e quando receber notificações',
        href: '/settings/notifications',
        icon: Bell,
        color: 'bg-yellow-100 text-yellow-600'
      },
      {
        name: 'Aparência',
        description: 'Personalize a interface da aplicação',
        href: '/settings/appearance',
        icon: Palette,
        color: 'bg-purple-100 text-purple-600'
      }
    ]
  },
  {
    title: 'Suporte',
    description: 'Obtenha ajuda e suporte técnico',
    items: [
      {
        name: 'Ajuda',
        description: 'Central de ajuda e documentação',
        href: '/help',
        icon: HelpCircle,
        color: 'bg-gray-100 text-gray-600'
      }
    ]
  }
]

export default function SettingsPage() {
  return (
    <DashboardLayout showSearchBar={false}>
      <div className="p-8">
        <div className="space-y-6">
          {/* Header */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center space-x-3">
              <div className="h-12 w-12 bg-blue-100 rounded-full flex items-center justify-center">
                <Settings className="h-6 w-6 text-blue-600" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">Configurações</h1>
                <p className="text-gray-600">Gerencie suas preferências e configurações da conta</p>
              </div>
            </div>
          </div>

          {/* Categorias de Configurações */}
          <div className="space-y-8">
            {settingsCategories.map((category) => (
              <div key={category.title} className="bg-white rounded-lg shadow-sm border border-gray-200">
                <div className="p-6 border-b border-gray-200">
                  <h2 className="text-lg font-semibold text-gray-900">{category.title}</h2>
                  <p className="text-sm text-gray-600 mt-1">{category.description}</p>
                </div>

                <div className="divide-y divide-gray-200">
                  {category.items.map((item) => (
                    <Link
                      key={item.name}
                      href={item.href}
                      className="block p-6 hover:bg-gray-50 transition-colors duration-200"
                    >
                      <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-4">
                          <div className={`h-10 w-10 rounded-lg flex items-center justify-center ${item.color}`}>
                            <item.icon className="h-5 w-5" />
                          </div>
                          <div>
                            <h3 className="text-sm font-medium text-gray-900">{item.name}</h3>
                            <p className="text-sm text-gray-500 mt-1">{item.description}</p>
                          </div>
                        </div>
                        <ChevronRight className="h-5 w-5 text-gray-400" />
                      </div>
                    </Link>
                  ))}
                </div>
              </div>
            ))}
          </div>

          {/* Informações Adicionais */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-6">
          <div className="flex items-start space-x-3">
            <div className="h-8 w-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0 mt-0.5">
              <HelpCircle className="h-4 w-4 text-blue-600" />
            </div>
            <div>
              <h3 className="text-sm font-medium text-blue-900">Precisa de ajuda?</h3>
              <p className="text-sm text-blue-700 mt-1">
                Se você tiver dúvidas sobre alguma configuração, consulte nossa{' '}
                <Link href="/help" className="font-medium underline hover:no-underline">
                  central de ajuda
                </Link>{' '}
                ou entre em contato com o suporte.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
    </DashboardLayout>
  )
}
