using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;

namespace Kickoffa.API.Application.Services.Checklists
{
    public class UpdateBriefingSectionService : IUpdateBriefingSectionService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IBriefingMediaFactory _briefingMediaFactory;
        private readonly IAddBriefingMediaService _addBriefingMediaService;
        private readonly ITipTapContentParserService _tipTapContentParserService;

        public UpdateBriefingSectionService(
            IFileUploadService fileUploadService,
            IBriefingMediaFactory briefingMediaFactory,
            IAddBriefingMediaService addBriefingMediaService,
            ITipTapContentParserService tipTapContentParserService)
        {
            _fileUploadService = fileUploadService;
            _briefingMediaFactory = briefingMediaFactory;
            _addBriefingMediaService = addBriefingMediaService;
            _tipTapContentParserService = tipTapContentParserService;
        }

        public Task<IBriefingSection> CreateAsync(long checklistId, BriefingSectionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(IBriefingSection briefingSection, BriefingSectionRequest updatedBriefingSectionRequest, CancellationToken cancellationToken)
        {
            // Gerenciar mídias antes de atualizar o conteúdo
            await UpdateBriefingSectionMediaAsync(briefingSection, updatedBriefingSectionRequest.ContentJson, cancellationToken);

            briefingSection.UpdateContent(updatedBriefingSectionRequest.ContentJson, updatedBriefingSectionRequest.ContentHtml);
        }

        /// <summary>
        /// Atualiza as mídias de uma seção de briefing baseado nas imagens do contentJson
        /// </summary>
        private async Task UpdateBriefingSectionMediaAsync(IBriefingSection briefingSection, string? newContentJson, CancellationToken cancellationToken)
        {
            // Extrair URLs de imagens do novo conteúdo
            var newImageUrls = _tipTapContentParserService.ExtractImageUrlsFromContentJson(newContentJson);

            // Obter URLs de imagens existentes na seção
            var existingImageUrls = briefingSection.Media.Select(x => x.Url);

            // Comparar URLs para identificar o que adicionar e remover
            var (urlsToAdd, urlsToRemove) = _tipTapContentParserService.CompareImageUrls(existingImageUrls, newImageUrls);

            // Adicionar novas mídias
            foreach (var urlToAdd in urlsToAdd)
            {

                _addBriefingMediaService.CreateAndAddBriefingMediaFromImageUrl(briefingSection, urlToAdd);
                /*
                try
                {
                    var fileName = _tipTapContentParserService.ExtractFileNameFromUrl(urlToAdd);
                    var media = _briefingMediaFactory.CreateBriefingMedia(
                        sectionId: briefingSection.Id,
                        fileName: fileName,
                        storagePath: urlToAdd,
                        url: urlToAdd,
                        contentType: "image/jpeg", // Tipo padrão, pode ser refinado
                        fileSize: 0 // Tamanho desconhecido
                    );

                    briefingSection.AddMedia(media);
                }
                catch (Exception)
                {
                    // Log do erro mas continua processando outras imagens
                    // TODO: Adicionar logging apropriado
                    continue;
                }
                */
            }

            // Remover mídias que não existem mais no conteúdo
            var mediaToRemove = briefingSection.Media
                .Where(m => urlsToRemove.Contains(m.Url))
                .ToList();

            foreach (var media in mediaToRemove)
            {
                try
                {
                    // Remover arquivo do S3
                    await _fileUploadService.DeleteFileAsync(media.S3FileKey, cancellationToken);

                    // Remover mídia da seção
                    briefingSection.RemoveMedia(media);
                }
                catch (Exception)
                {
                    // Log do erro mas continua processando outras mídias
                    // TODO: Adicionar logging apropriado
                    continue;
                }
            }
        }
    }
}