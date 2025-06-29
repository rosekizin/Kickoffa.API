using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.FileType;
using Kickoffa.API.Domain.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Controllers
{
	/// <summary>
	/// Controller para gerenciar tipos de arquivo
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize] // Sempre com autorização conforme solicitado
	public class FileTypeController : ControllerBase
	{
		private readonly IFileTypeService _fileTypeService;

		public FileTypeController(IFileTypeService fileTypeService)
		{
			_fileTypeService = fileTypeService;
		}

		/// <summary>
		/// Obtém todos os tipos de arquivo ativos disponíveis para upload
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo ativos</returns>
		[HttpGet]
		[ProducesResponseType(typeof(FileTypesSearchResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<FileTypesSearchResponse>> GetFileTypes(CancellationToken cancellationToken)
		{
			var result = await _fileTypeService.GetActiveFileTypesAsync(cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Busca tipos de arquivo por termo de pesquisa
		/// </summary>
		/// <param name="search">Termo para buscar em nome, extensão ou descrição</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo que correspondem à busca</returns>
		[HttpGet("search")]
		[ProducesResponseType(typeof(FileTypesSearchResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<FileTypesSearchResponse>> SearchFileTypes(
			[FromQuery] string? search,
			CancellationToken cancellationToken)
		{
			var result = await _fileTypeService.SearchFileTypesAsync(search, cancellationToken);
			return Ok(result);
		}

		/// <summary>
		/// Obtém tipos de arquivo por categoria
		/// </summary>
		/// <param name="category">Categoria dos arquivos</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de tipos de arquivo da categoria especificada</returns>
		[HttpGet("category/{category}")]
		[ProducesResponseType(typeof(FileTypesSearchResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<FileTypesSearchResponse>> GetFileTypesByCategory(
			FileTypeCategory category,
			CancellationToken cancellationToken)
		{
			var result = await _fileTypeService.GetFileTypesByCategoryAsync(category, cancellationToken);
			return Ok(result);
		}
	}
}
