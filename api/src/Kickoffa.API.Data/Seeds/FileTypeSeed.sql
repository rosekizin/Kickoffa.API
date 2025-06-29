-- Inserção de tipos de arquivo para o sistema Kickoffa.API
-- Executar após criação da tabela FileTypes
-- Banco postGreeSQL

INSERT INTO "FileTypes" 
("MimeType", "Extension", "DisplayName", "Description", "Category", "IsActive", "RecommendedMaxSizeMB", "DisplayOrder", "CreatedDateUtc", "LastUpdatedDateUtc") 
VALUES

-- IMAGENS
('image/jpeg', '.jpg', 'JPEG Image', 'Formato de imagem comprimida', 'Image', TRUE, 10, 1, NOW(), NOW()),
('image/jpeg', '.jpeg', 'JPEG Image', 'Formato de imagem comprimida', 'Image', TRUE, 10, 2, NOW(), NOW()),
('image/png', '.png', 'PNG Image', 'Formato de imagem sem perda', 'Image', TRUE, 15, 3, NOW(), NOW()),
('image/gif', '.gif', 'GIF Image', 'Formato de imagem animada', 'Image', TRUE, 5, 4, NOW(), NOW()),
('image/svg+xml', '.svg', 'SVG Vector', 'Gráfico vetorial escalável', 'Image', TRUE, 2, 5, NOW(), NOW()),
('image/webp', '.webp', 'WebP Image', 'Formato moderno de imagem', 'Image', TRUE, 8, 6, NOW(), NOW()),

-- DOCUMENTOS
('application/pdf', '.pdf', 'PDF Document', 'Documento portátil', 'Document', TRUE, 25, 10, NOW(), NOW()),
('application/msword', '.doc', 'Word Document', 'Documento do Microsoft Word', 'Document', TRUE, 20, 11, NOW(), NOW()),
('application/vnd.openxmlformats-officedocument.wordprocessingml.document', '.docx', 'Word Document', 'Documento do Microsoft Word (novo formato)', 'Document', TRUE, 20, 12, NOW(), NOW()),
('text/plain', '.txt', 'Text File', 'Arquivo de texto simples', 'Document', TRUE, 5, 13, NOW(), NOW()),
('text/rtf', '.rtf', 'Rich Text Format', 'Texto formatado', 'Document', TRUE, 10, 14, NOW(), NOW()),

-- PLANILHAS
('application/vnd.ms-excel', '.xls', 'Excel Spreadsheet', 'Planilha do Microsoft Excel', 'Spreadsheet', TRUE, 15, 20, NOW(), NOW()),
('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', '.xlsx', 'Excel Spreadsheet', 'Planilha do Microsoft Excel (novo formato)', 'Spreadsheet', TRUE, 15, 21, NOW(), NOW()),
('text/csv', '.csv', 'CSV File', 'Valores separados por vírgula', 'Spreadsheet', TRUE, 10, 22, NOW(), NOW()),

-- APRESENTAÇÕES
('application/vnd.ms-powerpoint', '.ppt', 'PowerPoint Presentation', 'Apresentação do Microsoft PowerPoint', 'Presentation', TRUE, 50, 30, NOW(), NOW()),
('application/vnd.openxmlformats-officedocument.presentationml.presentation', '.pptx', 'PowerPoint Presentation', 'Apresentação do Microsoft PowerPoint (novo formato)', 'Presentation', TRUE, 50, 31, NOW(), NOW()),

-- ÁUDIO
('audio/mpeg', '.mp3', 'MP3 Audio', 'Arquivo de áudio comprimido', 'Audio', TRUE, 50, 40, NOW(), NOW()),
('audio/wav', '.wav', 'WAV Audio', 'Arquivo de áudio sem compressão', 'Audio', TRUE, 100, 41, NOW(), NOW()),
('audio/ogg', '.ogg', 'OGG Audio', 'Arquivo de áudio Ogg Vorbis', 'Audio', TRUE, 50, 42, NOW(), NOW()),

-- VÍDEO
('video/mp4', '.mp4', 'MP4 Video', 'Vídeo em formato MP4', 'Video', TRUE, 500, 50, NOW(), NOW()),
('video/avi', '.avi', 'AVI Video', 'Vídeo em formato AVI', 'Video', TRUE, 500, 51, NOW(), NOW()),
('video/quicktime', '.mov', 'QuickTime Video', 'Vídeo QuickTime', 'Video', TRUE, 500, 52, NOW(), NOW()),

-- DESIGN
('image/vnd.adobe.photoshop', '.psd', 'Photoshop Document', 'Arquivo do Adobe Photoshop', 'Design', TRUE, 200, 60, NOW(), NOW()),
('application/postscript', '.ai', 'Adobe Illustrator', 'Arquivo do Adobe Illustrator', 'Design', TRUE, 100, 61, NOW(), NOW()),
('application/x-indesign', '.indd', 'InDesign Document', 'Arquivo do Adobe InDesign', 'Design', TRUE, 150, 62, NOW(), NOW()),

-- ARQUIVOS COMPRIMIDOS
('application/zip', '.zip', 'ZIP Archive', 'Arquivo comprimido ZIP', 'Archive', TRUE, 100, 70, NOW(), NOW()),
('application/x-rar-compressed', '.rar', 'RAR Archive', 'Arquivo comprimido RAR', 'Archive', TRUE, 100, 71, NOW(), NOW()),

-- CÓDIGO
('text/html', '.html', 'HTML File', 'Arquivo HTML', 'Code', TRUE, 5, 80, NOW(), NOW()),
('text/css', '.css', 'CSS File', 'Folha de estilo CSS', 'Code', TRUE, 5, 81, NOW(), NOW()),
('application/javascript', '.js', 'JavaScript File', 'Arquivo JavaScript', 'Code', TRUE, 5, 82, NOW(), NOW()),
('text/x-csharp', '.cs', 'C# File', 'Arquivo C#', 'Code', TRUE, 5, 83, NOW(), NOW()),
('text/x-java-source', '.java', 'Java File', 'Arquivo Java', 'Code', TRUE, 5, 84, NOW(), NOW()),
('text/x-python', '.py', 'Python File', 'Arquivo Python', 'Code', TRUE, 5, 85, NOW(), NOW()),
('text/x-go', '.go', 'Go File', 'Arquivo Go', 'Code', TRUE, 5, 86, NOW(), NOW()),
('text/x-ruby', '.rb', 'Ruby File', 'Arquivo Ruby', 'Code', TRUE, 5, 87, NOW(), NOW()),
('text/x-php', '.php', 'PHP File', 'Arquivo PHP', 'Code', TRUE, 5, 88, NOW(), NOW()),
('text/x-c', '.c', 'C File', 'Arquivo C', 'Code', TRUE, 5, 89, NOW(), NOW()),
('text/x-c++src', '.cpp', 'C++ File', 'Arquivo C++', 'Code', TRUE, 5, 90, NOW(), NOW()),
('text/x-typescript', '.ts', 'TypeScript File', 'Arquivo TypeScript', 'Code', TRUE, 5, 91, NOW(), NOW()),
('application/json', '.json', 'JSON File', 'Arquivo JSON', 'Code', TRUE, 5, 92, NOW(), NOW()),
('application/x-yaml', '.yaml', 'YAML File', 'Arquivo YAML', 'Code', TRUE, 5, 93, NOW(), NOW()),
('application/x-yaml', '.yml', 'YAML File', 'Arquivo YAML', 'Code', TRUE, 5, 94, NOW(), NOW()),
('application/xml', '.xml', 'XML File', 'Arquivo XML', 'Code', TRUE, 5, 95, NOW(), NOW()),
('application/sql', '.sql', 'SQL File', 'Arquivo SQL', 'Code', TRUE, 5, 96, NOW(), NOW()),
('text/x-perl', '.pl', 'Perl File', 'Arquivo Perl', 'Code', TRUE, 5, 97, NOW(), NOW()),
('text/x-rust', '.rs', 'Rust File', 'Arquivo Rust', 'Code', TRUE, 5, 98, NOW(), NOW()),
('text/x-swift', '.swift', 'Swift File', 'Arquivo Swift', 'Code', TRUE, 5, 99, NOW(), NOW()),

-- FONTES
('font/ttf', '.ttf', 'TrueType Font', 'Fonte TrueType', 'Font', TRUE, 5, 100, NOW(), NOW()),
('font/otf', '.otf', 'OpenType Font', 'Fonte OpenType', 'Font', TRUE, 5, 101, NOW(), NOW());

-- Verificar inserção
SELECT COUNT(*) AS "TotalFileTypes" FROM "FileTypes";
SELECT "Category", COUNT(*) AS "Count" FROM "FileTypes" GROUP BY "Category" ORDER BY "Category";

