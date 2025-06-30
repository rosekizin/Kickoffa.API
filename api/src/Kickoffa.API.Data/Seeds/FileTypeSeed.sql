-- Inserção de tipos de arquivo para o sistema Kickoffa.API
-- Executar após criação da tabela FileTypes
-- Banco PostgreSQL

INSERT INTO "FileTypes"
("MimeType", "Extension", "DisplayName", "Description", "Category", "IsActive", "RecommendedMaxSizeMB", "DisplayOrder", "CreatedDateUtc", "LastUpdatedDateUtc")
VALUES

-- IMAGENS
('image/jpeg', '.jpg', 'Imagem JPEG', 'Formato de imagem comprimida', 'Image', TRUE, 10, 1, NOW(), NOW()),
('image/jpeg', '.jpeg', 'Imagem JPEG', 'Formato de imagem comprimida', 'Image', TRUE, 10, 2, NOW(), NOW()),
('image/png', '.png', 'Imagem PNG', 'Formato de imagem sem perda', 'Image', TRUE, 15, 3, NOW(), NOW()),
('image/gif', '.gif', 'Imagem GIF', 'Formato de imagem animada', 'Image', TRUE, 5, 4, NOW(), NOW()),
('image/svg+xml', '.svg', 'Vetor SVG', 'Gráfico vetorial escalável', 'Image', TRUE, 2, 5, NOW(), NOW()),
('image/webp', '.webp', 'Imagem WebP', 'Formato moderno de imagem', 'Image', TRUE, 8, 6, NOW(), NOW()),
('image/bmp', '.bmp', 'Imagem BMP', 'Formato bitmap do Windows', 'Image', TRUE, 20, 7, NOW(), NOW()),
('image/tiff', '.tiff', 'Imagem TIFF', 'Formato de imagem de alta qualidade', 'Image', TRUE, 50, 8, NOW(), NOW()),
('image/tiff', '.tif', 'Imagem TIFF', 'Formato de imagem de alta qualidade', 'Image', TRUE, 50, 9, NOW(), NOW()),

-- DOCUMENTOS
('application/pdf', '.pdf', 'Documento PDF', 'Documento portátil', 'Document', TRUE, 25, 20, NOW(), NOW()),
('application/msword', '.doc', 'Documento Word', 'Documento do Microsoft Word', 'Document', TRUE, 20, 21, NOW(), NOW()),
('application/vnd.openxmlformats-officedocument.wordprocessingml.document', '.docx', 'Documento Word', 'Documento do Microsoft Word (novo formato)', 'Document', TRUE, 20, 22, NOW(), NOW()),
('text/plain', '.txt', 'Arquivo de Texto', 'Arquivo de texto simples', 'Document', TRUE, 5, 23, NOW(), NOW()),
('text/rtf', '.rtf', 'Texto Formatado RTF', 'Texto formatado', 'Document', TRUE, 10, 24, NOW(), NOW()),

-- PLANILHAS
('application/vnd.ms-excel', '.xls', 'Planilha Excel', 'Planilha do Microsoft Excel', 'Spreadsheet', TRUE, 15, 30, NOW(), NOW()),
('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', '.xlsx', 'Planilha Excel', 'Planilha do Microsoft Excel (novo formato)', 'Spreadsheet', TRUE, 15, 31, NOW(), NOW()),
('text/csv', '.csv', 'Arquivo CSV', 'Valores separados por vírgula', 'Spreadsheet', TRUE, 10, 32, NOW(), NOW()),

-- APRESENTAÇÕES
('application/vnd.ms-powerpoint', '.ppt', 'Apresentação PowerPoint', 'Apresentação do Microsoft PowerPoint', 'Presentation', TRUE, 50, 40, NOW(), NOW()),
('application/vnd.openxmlformats-officedocument.presentationml.presentation', '.pptx', 'Apresentação PowerPoint', 'Apresentação do Microsoft PowerPoint (novo formato)', 'Presentation', TRUE, 50, 41, NOW(), NOW()),

-- ÁUDIO
('audio/mpeg', '.mp3', 'Áudio MP3', 'Arquivo de áudio comprimido', 'Audio', TRUE, 50, 50, NOW(), NOW()),
('audio/wav', '.wav', 'Áudio WAV', 'Arquivo de áudio sem compressão', 'Audio', TRUE, 100, 51, NOW(), NOW()),
('audio/ogg', '.ogg', 'Áudio OGG', 'Arquivo de áudio Ogg Vorbis', 'Audio', TRUE, 50, 52, NOW(), NOW()),
('audio/aac', '.aac', 'Áudio AAC', 'Arquivo de áudio AAC', 'Audio', TRUE, 30, 53, NOW(), NOW()),
('audio/flac', '.flac', 'Áudio FLAC', 'Arquivo de áudio sem perda', 'Audio', TRUE, 150, 54, NOW(), NOW()),

-- VÍDEO
('video/mp4', '.mp4', 'Vídeo MP4', 'Vídeo em formato MP4', 'Video', TRUE, 500, 60, NOW(), NOW()),
('video/avi', '.avi', 'Vídeo AVI', 'Vídeo em formato AVI', 'Video', TRUE, 500, 61, NOW(), NOW()),
('video/quicktime', '.mov', 'Vídeo QuickTime', 'Vídeo QuickTime', 'Video', TRUE, 500, 62, NOW(), NOW()),
('video/x-msvideo', '.wmv', 'Vídeo WMV', 'Vídeo Windows Media', 'Video', TRUE, 500, 63, NOW(), NOW()),
('video/webm', '.webm', 'Vídeo WebM', 'Vídeo para web', 'Video', TRUE, 300, 64, NOW(), NOW()),

-- DESIGN
('image/vnd.adobe.photoshop', '.psd', 'Arquivo Photoshop', 'Arquivo do Adobe Photoshop', 'Design', TRUE, 200, 70, NOW(), NOW()),
('application/postscript', '.ai', 'Arquivo Illustrator', 'Arquivo do Adobe Illustrator', 'Design', TRUE, 100, 71, NOW(), NOW()),
('application/x-indesign', '.indd', 'Arquivo InDesign', 'Arquivo do Adobe InDesign', 'Design', TRUE, 150, 72, NOW(), NOW()),
('application/x-coreldraw', '.cdr', 'Arquivo CorelDRAW', 'Arquivo do CorelDRAW', 'Design', TRUE, 100, 73, NOW(), NOW()),
('application/x-sketch', '.sketch', 'Arquivo Sketch', 'Arquivo do Sketch', 'Design', TRUE, 50, 74, NOW(), NOW()),
('application/x-figma', '.fig', 'Arquivo Figma', 'Arquivo do Figma', 'Design', TRUE, 30, 75, NOW(), NOW()),
('application/x-adobe-xd', '.xd', 'Arquivo Adobe XD', 'Arquivo do Adobe XD', 'Design', TRUE, 50, 76, NOW(), NOW()),
('application/x-adobe-after-effects', '.aep', 'Projeto After Effects', 'Projeto do Adobe After Effects', 'Design', TRUE, 500, 77, NOW(), NOW()),
('application/x-adobe-premiere', '.prproj', 'Projeto Premiere', 'Projeto do Adobe Premiere Pro', 'Design', TRUE, 100, 78, NOW(), NOW()),

-- ARQUIVOS COMPRIMIDOS
('application/zip', '.zip', 'Arquivo ZIP', 'Arquivo comprimido ZIP', 'Archive', TRUE, 100, 80, NOW(), NOW()),
('application/x-rar-compressed', '.rar', 'Arquivo RAR', 'Arquivo comprimido RAR', 'Archive', TRUE, 100, 81, NOW(), NOW()),
('application/x-7z-compressed', '.7z', 'Arquivo 7-Zip', 'Arquivo comprimido 7-Zip', 'Archive', TRUE, 100, 82, NOW(), NOW()),
('application/gzip', '.gz', 'Arquivo GZIP', 'Arquivo comprimido GZIP', 'Archive', TRUE, 50, 83, NOW(), NOW()),
('application/x-tar', '.tar', 'Arquivo TAR', 'Arquivo TAR', 'Archive', TRUE, 100, 84, NOW(), NOW()),
('application/x-bzip2', '.bz2', 'Arquivo BZIP2', 'Arquivo comprimido BZIP2', 'Archive', TRUE, 50, 85, NOW(), NOW()),

-- CÓDIGO
('text/html', '.html', 'Arquivo HTML', 'Arquivo HTML', 'Code', TRUE, 5, 90, NOW(), NOW()),
('text/css', '.css', 'Arquivo CSS', 'Folha de estilo CSS', 'Code', TRUE, 5, 91, NOW(), NOW()),
('application/javascript', '.js', 'Arquivo JavaScript', 'Arquivo JavaScript', 'Code', TRUE, 5, 92, NOW(), NOW()),
('text/x-csharp', '.cs', 'Arquivo C#', 'Arquivo C#', 'Code', TRUE, 5, 93, NOW(), NOW()),
('text/x-java-source', '.java', 'Arquivo Java', 'Arquivo Java', 'Code', TRUE, 5, 94, NOW(), NOW()),
('text/x-python', '.py', 'Arquivo Python', 'Arquivo Python', 'Code', TRUE, 5, 95, NOW(), NOW()),
('text/x-go', '.go', 'Arquivo Go', 'Arquivo Go', 'Code', TRUE, 5, 96, NOW(), NOW()),
('text/x-ruby', '.rb', 'Arquivo Ruby', 'Arquivo Ruby', 'Code', TRUE, 5, 97, NOW(), NOW()),
('text/x-php', '.php', 'Arquivo PHP', 'Arquivo PHP', 'Code', TRUE, 5, 98, NOW(), NOW()),
('text/x-c', '.c', 'Arquivo C', 'Arquivo C', 'Code', TRUE, 5, 99, NOW(), NOW()),
('text/x-c++src', '.cpp', 'Arquivo C++', 'Arquivo C++', 'Code', TRUE, 5, 100, NOW(), NOW()),
('text/x-typescript', '.ts', 'Arquivo TypeScript', 'Arquivo TypeScript', 'Code', TRUE, 5, 101, NOW(), NOW()),
('application/json', '.json', 'Arquivo JSON', 'Arquivo JSON', 'Code', TRUE, 5, 102, NOW(), NOW()),
('application/x-yaml', '.yaml', 'Arquivo YAML', 'Arquivo YAML', 'Code', TRUE, 5, 103, NOW(), NOW()),
('application/x-yaml', '.yml', 'Arquivo YAML', 'Arquivo YAML', 'Code', TRUE, 5, 104, NOW(), NOW()),
('application/xml', '.xml', 'Arquivo XML', 'Arquivo XML', 'Code', TRUE, 5, 105, NOW(), NOW()),
('application/sql', '.sql', 'Arquivo SQL', 'Arquivo SQL', 'Code', TRUE, 5, 106, NOW(), NOW()),
('text/x-perl', '.pl', 'Arquivo Perl', 'Arquivo Perl', 'Code', TRUE, 5, 107, NOW(), NOW()),
('text/x-rust', '.rs', 'Arquivo Rust', 'Arquivo Rust', 'Code', TRUE, 5, 108, NOW(), NOW()),
('text/x-swift', '.swift', 'Arquivo Swift', 'Arquivo Swift', 'Code', TRUE, 5, 109, NOW(), NOW()),
('text/x-kotlin', '.kt', 'Arquivo Kotlin', 'Arquivo Kotlin', 'Code', TRUE, 5, 110, NOW(), NOW()),
('text/x-dart', '.dart', 'Arquivo Dart', 'Arquivo Dart', 'Code', TRUE, 5, 111, NOW(), NOW()),

-- FONTES
('font/ttf', '.ttf', 'Fonte TrueType', 'Fonte TrueType', 'Font', TRUE, 5, 120, NOW(), NOW()),
('font/otf', '.otf', 'Fonte OpenType', 'Fonte OpenType', 'Font', TRUE, 5, 121, NOW(), NOW()),
('font/woff', '.woff', 'Fonte WOFF', 'Fonte Web Open Font Format', 'Font', TRUE, 2, 122, NOW(), NOW()),
('font/woff2', '.woff2', 'Fonte WOFF2', 'Fonte Web Open Font Format 2', 'Font', TRUE, 2, 123, NOW(), NOW());

-- Verificar inserção
SELECT COUNT(*) AS "TotalFileTypes" FROM "FileTypes";
SELECT "Category", COUNT(*) AS "Count" FROM "FileTypes" GROUP BY "Category" ORDER BY "Category";

