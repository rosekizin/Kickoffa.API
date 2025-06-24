# Kickoffa Frontend

Frontend da aplicação Kickoffa - Portal de onboarding para freelancers.

## Stack Tecnológica

- **Framework**: Next.js 15 com React 19
- **Linguagem**: TypeScript
- **Estilização**: Tailwind CSS + shadcn/ui
- **Editor Rich Text**: TipTap
- **Gerenciamento de Estado**: Zustand
- **Formulários**: React Hook Form + Zod
- **HTTP Client**: Axios
- **Gerenciamento de Estado do Servidor**: TanStack Query

## Estrutura do Projeto

```
src/
├── app/                    # Rotas Next.js (App Router)
├── components/             # Componentes React
│   ├── ui/                 # Componentes de UI básicos (shadcn)
│   ├── briefing/           # Componentes relacionados ao briefing
│   └── shared/             # Componentes compartilhados
├── lib/                    # Utilitários e helpers
│   ├── api.ts              # Cliente API
│   └── utils.ts            # Funções utilitárias
├── store/                  # Estado global (Zustand)
├── types/                  # Definições de tipos TypeScript
└── hooks/                  # Custom hooks
```

## Configuração e Execução

### Pré-requisitos

- Node.js 18+
- npm ou yarn

### Instalação

```bash
# Instalar dependências
npm install

# Configurar variáveis de ambiente
cp .env.example .env.local
```

### Desenvolvimento

```bash
# Executar servidor de desenvolvimento
npm run dev
```

Abra [http://localhost:3000](http://localhost:3000) no seu navegador.

### Build para Produção

```bash
# Build da aplicação
npm run build

# Executar versão de produção
npm start
```

## Funcionalidades Principais

### Portal de Onboarding
- Link único sem necessidade de login para clientes
- Coleta estruturada de informações antes do projeto
- Interface mobile-first com drag-and-drop

### Editor de Briefing
- Editor rich text com TipTap (interface moderna)
- Suporte a imagens, links e formatação avançada
- Upload direto de mídia integrado
- Exportação em JSON e HTML

### Checklist Interativo
- Itens configuráveis (checkbox, upload, texto, assinatura)
- Barra de progresso em tempo real
- Validação de arquivos e campos obrigatórios

### Notificações Automáticas
- Lembretes via WhatsApp/e-mail para clientes
- Alertas instantâneos para freelancers
- Acompanhamento de prazos

### Prova de Escopo
- PDF de evidência com hash e timestamp
- ZIP com todos os uploads
- Proteção legal para freelancers

## Integração com Backend

A aplicação se conecta com a API .NET localizada em `http://localhost:5084` por padrão.

### Configuração da API

Edite o arquivo `.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5084
```

## Componentes UI

O projeto utiliza shadcn/ui para componentes base. Para adicionar novos componentes:

```bash
npx shadcn-ui@latest add [component-name]
```

## Scripts Disponíveis

- `npm run dev` - Servidor de desenvolvimento
- `npm run build` - Build para produção
- `npm run start` - Executar build de produção
- `npm run lint` - Executar ESLint

## Contribuição

1. Faça fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request
