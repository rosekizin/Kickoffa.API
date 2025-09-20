# Teste de Upload de Imagens - AWS S3

## Visão Geral

Este documento descreve como testar a funcionalidade de upload de imagens implementada no TipTap com integração AWS S3.

## Configuração Necessária

### 1. Backend (API)

Certifique-se de que o backend está configurado corretamente:

```json
// appsettings.Development.json
{
  "AWS": {
    "S3": {
      "BucketName": "seu-bucket-de-desenvolvimento",
      "Region": "us-east-1",
      "AccessKey": "sua-access-key",
      "SecretKey": "sua-secret-key",
      "BaseUrl": "https://seu-bucket.s3.us-east-1.amazonaws.com",
      "UseLocalStack": false
    }
  }
}
```

### 2. Frontend

1. Inicie o backend: `dotnet run` na pasta `api/src/Kickoffa.API`
2. Inicie o frontend: `npm run dev` na pasta `web`
3. Acesse: `http://localhost:3000/test-upload`

## Funcionalidades Implementadas

### ✅ Upload via Botão
- Clique no ícone de imagem na toolbar
- Selecione uma imagem do computador
- Observe o spinner no botão durante upload

### ✅ Upload via Drag & Drop
- Arraste uma imagem do explorador de arquivos
- Solte diretamente no editor
- Imagem aparece na posição do drop

### ✅ Upload via Paste (Ctrl+V)
- Copie uma imagem de qualquer lugar
- Cole no editor usando Ctrl+V
- Imagem inserida na posição do cursor

### ✅ Indicadores Visuais
- **Durante upload:** Borda tracejada azul + animação pulse
- **Progress:** Indicador no canto superior esquerdo
- **Erro:** Mensagem vermelha com botão para fechar
- **Sucesso:** Imagem normal sem indicadores

### ✅ Validações
- Tipos aceitos: JPEG, PNG, GIF, WebP, SVG
- Tamanho máximo: 10MB
- Rejeita arquivos vazios

## Como Testar

### 1. Teste Básico
1. Acesse `http://localhost:3000/test-upload`
2. Certifique-se de estar autenticado
3. Clique no botão de imagem na toolbar
4. Selecione uma imagem pequena (< 1MB)
5. Observe o upload e resultado

### 2. Teste de Drag & Drop
1. Abra o explorador de arquivos
2. Arraste uma imagem para o editor
3. Solte na posição desejada
4. Verifique se a imagem aparece corretamente

### 3. Teste de Paste
1. Copie uma imagem (Ctrl+C) de outro aplicativo
2. Clique no editor para posicionar cursor
3. Cole a imagem (Ctrl+V)
4. Verifique se foi inserida na posição correta

### 4. Teste de Validação
1. Tente fazer upload de arquivo não-imagem (.txt, .pdf)
2. Tente fazer upload de imagem muito grande (> 10MB)
3. Verifique se as mensagens de erro aparecem

### 5. Teste de Múltiplas Imagens
1. Faça upload de várias imagens simultaneamente
2. Observe se todas são processadas corretamente
3. Verifique se não há conflitos

## Estrutura de Arquivos Criados

```
web/src/
├── services/
│   └── fileUpload.service.ts          # Serviço para comunicação com API
├── hooks/
│   └── use-image-upload.ts            # Hook para gerenciar estado de upload
├── components/briefing/
│   ├── briefing-editor.tsx            # Editor atualizado com upload S3
│   ├── tiptap-upload-plugin.ts        # Plugin TipTap para upload automático
│   └── upload-test.tsx                # Componente de teste
└── app/test-upload/
    └── page.tsx                       # Página de teste
```

## Debugging

### Console do Navegador
Observe as mensagens no console:
- `🚀 API Request: POST /api/fileupload/image/briefing`
- `✅ API Success: POST /api/fileupload/image/briefing - Status: 200`
- `Upload iniciado: filename.jpg`
- `Upload concluído: filename.jpg https://bucket.s3.amazonaws.com/...`

### Network Tab
Verifique as requisições HTTP:
- Método: `POST`
- URL: `/api/fileupload/image/briefing`
- Content-Type: `multipart/form-data`
- Status: `200 OK`

### Possíveis Problemas

1. **Erro 401 (Unauthorized)**
   - Solução: Faça login na aplicação

2. **Erro 400 (Bad Request)**
   - Causa: Arquivo inválido ou muito grande
   - Solução: Use imagem válida < 10MB

3. **Erro 500 (Internal Server Error)**
   - Causa: Configuração AWS S3 incorreta
   - Solução: Verifique credenciais no appsettings.json

4. **Upload não inicia**
   - Causa: Plugin não registrado
   - Solução: Verifique se está em modo de edição

5. **Imagem não aparece**
   - Causa: URL do S3 incorreta
   - Solução: Verifique BaseUrl nas configurações

## URLs de Teste

- **Página de teste:** `http://localhost:3000/test-upload`
- **Editor principal:** `http://localhost:3000` (seção de briefing)
- **API Health:** `http://localhost:5084/api/fileupload` (deve retornar 401)

## Próximos Passos

Após confirmar que tudo funciona:

1. ✅ Remover URLs blob antigas do código
2. ✅ Implementar limpeza de arquivos órfãos (opcional)
3. ✅ Configurar bucket S3 de produção
4. ✅ Implementar compressão de imagens (opcional)
5. ✅ Adicionar suporte a mais formatos (opcional)

## Suporte

Se encontrar problemas:

1. Verifique o console do navegador
2. Verifique os logs do backend
3. Confirme as configurações AWS S3
4. Teste com imagens pequenas primeiro
5. Verifique se está autenticado
