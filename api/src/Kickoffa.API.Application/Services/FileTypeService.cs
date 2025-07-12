using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Services
{
	/// <summary>
	/// Implementação do serviço de tipos de arquivo
	/// </summary>
	public class FileTypeService : IFileTypeService
	{
		private readonly IFileTypeRepository _fileTypeRepository;

		public FileTypeService(IFileTypeRepository fileTypeRepository)
		{
			_fileTypeRepository = fileTypeRepository;
		}

		public async Task<FileTypesSearchResponse> GetActiveFileTypesAsync(CancellationToken cancellationToken)
		{
			var fileTypes = await _fileTypeRepository.GetActiveFileTypesAsync(cancellationToken);
			var fileTypeResponses = fileTypes.Select(MapToResponse).ToList();

			return new FileTypesSearchResponse(fileTypeResponses, fileTypeResponses.Count);
		}

		public async Task<FileTypesSearchResponse> SearchFileTypesAsync(string? searchTerm, CancellationToken cancellationToken)
		{
			var fileTypes = string.IsNullOrWhiteSpace(searchTerm)
				? await _fileTypeRepository.GetActiveFileTypesAsync(cancellationToken)
				: await _fileTypeRepository.SearchFileTypesAsync(searchTerm, cancellationToken);

			var fileTypeResponses = fileTypes.Select(MapToResponse).ToList();

			return new FileTypesSearchResponse(fileTypeResponses, fileTypeResponses.Count);
		}

		public async Task<FileTypesSearchResponse> GetFileTypesByCategoryAsync(FileTypeCategory category, CancellationToken cancellationToken)
		{
			var fileTypes = await _fileTypeRepository.GetFileTypesByCategoryAsync(category, cancellationToken);
			var fileTypeResponses = fileTypes.Select(MapToResponse).ToList();

			return new FileTypesSearchResponse(fileTypeResponses, fileTypeResponses.Count);
		}

		private static FileTypeResponse MapToResponse(IFileType fileType)
		{
			return new FileTypeResponse(
				fileType.Id,
				fileType.MimeType,
				fileType.Extension,
				fileType.DisplayName,
				fileType.Description,
				Enum.GetName(fileType.Category),
				fileType.RecommendedMaxSizeMB
			);
		}
	}
}