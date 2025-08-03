using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Domain.Interfaces.Models;
using Microsoft.Extensions.Logging;

namespace Kickoffa.API.Application.Services.Checklists
{
    public class AddBriefingMediaService : IAddBriefingMediaService
    {
        private const string PREFFIX_TO_REMOVE = "/api/images";

        private static readonly Dictionary<string, string> _imageContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".bmp", "image/bmp" },
            { ".webp", "image/webp" },
            { ".tiff", "image/tiff" },
            { ".svg", "image/svg+xml" },
            { ".ico", "image/x-icon" },
            { ".avif", "image/avif" }
        };

        private readonly ILogger<AddBriefingMediaService> _logger;
        private readonly IBriefingMediaFactory _briefingMediaFactory;
        private readonly ITipTapContentParserService _tipTapContentParserService;

        public AddBriefingMediaService(
            ILogger<AddBriefingMediaService> logger,
            IBriefingMediaFactory briefingMediaFactory,
            ITipTapContentParserService tipTapContentParserService)
        {
            _logger = logger;
            _briefingMediaFactory = briefingMediaFactory;
            _tipTapContentParserService = tipTapContentParserService;
        }

        public void CreateAndAddBriefingMediaFromImageUrl(IBriefingSection section, string imageUrl)
        {
            CreateAndAddBriefingMedia(section, imageUrl);
        }

        /// <summary>
        /// Processa as URLs de imagens e adiciona como media na seção
        /// </summary>
        /// <param name="section">Seção de briefing</param>
        /// <param name="imageUrls">Lista de URLs de imagens</param>
        public void CreateAndAddBriefingMediaFromImageUrls(IBriefingSection section, List<string> imageUrls)
        {
            foreach (var imageUrl in imageUrls)
            {
                CreateAndAddBriefingMedia(section, imageUrl);
            }
        }

        private void CreateAndAddBriefingMedia(IBriefingSection section, string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return;

                // Extrair informações da URL
                var fileName = _tipTapContentParserService.ExtractFileNameFromUrl(imageUrl);

                // Criar BriefingMedia usando o factory
                var media = _briefingMediaFactory.CreateBriefingMedia(
                    sectionId: section.Id, // Será 0 inicialmente, será atualizado quando persistido
                    fileName: fileName,
                    storagePath: GetPathFromUrl(imageUrl), // Usar a URL como storage path por enquanto
                    url: imageUrl,
                    contentType: GetContentTypeFromImageUrl(imageUrl),
                    //contentType: "image/jpeg", // Tipo padrão, pode ser refinado posteriormente
                    fileSize: 0 // Tamanho desconhecido por enquanto
                );

                // Adicionar media à seção
                section.AddMedia(media);
            }
            catch (Exception ex)
            {
                // Se houver erro ao processar uma imagem específica, continuar com as outras
                _logger.LogError(ex, "Ocorreu algum erro ao criar o BriefingMedia para imageUrl {ImageUrl}, na section {SectionId}", imageUrl, section?.Id);
            }
        }

        private static string GetPathFromUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return uri.AbsolutePath.Replace(PREFFIX_TO_REMOVE, string.Empty);

            return string.Empty;
        }

        private static string GetContentTypeFromImageUrl(string url)
        {
            var uri = new Uri(url);
            var extension = Path.GetExtension(uri.LocalPath); // ex: ".jpg"

            _imageContentTypes.TryGetValue(extension, out var contentType);
            return contentType!;
        }
    }
}