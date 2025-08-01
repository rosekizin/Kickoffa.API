using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Services.Checklists
{
    public class AddBriefingMediaService : IAddBriefingMediaService
    {
        private const string PREFFIX_TO_REMOVE = "/api/images";

        private readonly IBriefingMediaFactory _briefingMediaFactory;
        private readonly ITipTapContentParserService _tipTapContentParserService;

        public AddBriefingMediaService(
            IBriefingMediaFactory briefingMediaFactory,
            ITipTapContentParserService tipTapContentParserService)
        {
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
                // Extrair informações da URL
                var fileName = _tipTapContentParserService.ExtractFileNameFromUrl(imageUrl);

                // Criar BriefingMedia usando o factory
                var media = _briefingMediaFactory.CreateBriefingMedia(
                    sectionId: section.Id, // Será 0 inicialmente, será atualizado quando persistido
                    fileName: fileName,
                    storagePath: GetPathFromUrl(imageUrl), // Usar a URL como storage path por enquanto
                    url: imageUrl,
                    contentType: "image/jpeg", // Tipo padrão, pode ser refinado posteriormente
                    fileSize: 0 // Tamanho desconhecido por enquanto
                );

                // Adicionar media à seção
                section.AddMedia(media);
            }
            catch (Exception)
            {
                // Se houver erro ao processar uma imagem específica, continuar com as outras
                //continue;
            }
        }

        private static string GetPathFromUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return uri.AbsolutePath.Replace(PREFFIX_TO_REMOVE, string.Empty);

            return string.Empty;
        }
    }
}