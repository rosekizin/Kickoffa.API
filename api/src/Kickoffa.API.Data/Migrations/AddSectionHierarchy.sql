-- Migration para implementar hierarquia de Section
-- Executar após atualização do código

-- 1. Verificar se a coluna Type já existe como string
-- Se não existir ou for enum, precisamos ajustar

-- Primeiro, vamos verificar a estrutura atual
-- SELECT column_name, data_type FROM information_schema.columns 
-- WHERE table_name = 'Sections' AND column_name = 'Type';

-- 2. Se a coluna Type não for string, vamos ajustá-la
-- ALTER TABLE "Sections" ALTER COLUMN "Type" TYPE VARCHAR(50);

-- 3. Atualizar valores existentes para usar os discriminadores corretos
UPDATE "Sections" 
SET "Type" = 'Briefing' 
WHERE "Type" = '0' OR "Type" = 'Briefing';

UPDATE "Sections" 
SET "Type" = 'Checklist' 
WHERE "Type" = '1' OR "Type" = 'Checklist';

-- 4. Verificar se há registros com tipos inválidos
-- SELECT "Id", "Type" FROM "Sections" WHERE "Type" NOT IN ('Briefing', 'Checklist');

-- 5. Adicionar constraint para garantir apenas valores válidos
ALTER TABLE "Sections" 
ADD CONSTRAINT "CK_Sections_Type" 
CHECK ("Type" IN ('Briefing', 'Checklist'));

-- 6. Criar índice no discriminador se não existir
CREATE INDEX IF NOT EXISTS "IX_Sections_Type" ON "Sections" ("Type");

-- 7. Comentários nas colunas para documentação
COMMENT ON COLUMN "Sections"."Type" IS 'Discriminador para hierarquia TPH: Briefing ou Checklist';
COMMENT ON COLUMN "Sections"."ContentJson" IS 'Conteúdo JSON - apenas para BriefingSection';
COMMENT ON COLUMN "Sections"."ContentHtml" IS 'Conteúdo HTML - apenas para BriefingSection';
COMMENT ON COLUMN "Sections"."ContentLastUpdated" IS 'Data última atualização conteúdo - apenas para BriefingSection';

-- 8. Verificação final
SELECT 
    "Type",
    COUNT(*) as "Count",
    COUNT(CASE WHEN "ContentJson" IS NOT NULL THEN 1 END) as "WithContentJson",
    COUNT(CASE WHEN "ContentHtml" IS NOT NULL THEN 1 END) as "WithContentHtml"
FROM "Sections" 
GROUP BY "Type";
