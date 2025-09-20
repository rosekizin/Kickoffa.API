using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Microsoft.AspNetCore.Http;

namespace Kickoffa.API.Application.Interfaces
{
	/// <summary>
	/// Serviço para upload de arquivos para AWS S3
	/// </summary>
	public interface IFileUploadService
	{
		/// <summary>
		/// Faz upload de uma imagem para o S3 e retorna a URL
		/// </summary>
		/// <param name="file">Arquivo de imagem</param>
		/// <param name="folder">Pasta no S3 onde salvar o arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Result contendo a URL da imagem no S3 ou erro</returns>
		Task<IResult<string>> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken);

		/// <summary>
		/// Faz upload de um arquivo genérico para o S3 e retorna a URL
		/// </summary>
		/// <param name="file">Arquivo</param>
		/// <param name="folder">Pasta no S3 onde salvar o arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Result contendo a URL do arquivo no S3 ou erro</returns>
		Task<IResult<string>> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken);

        /// <summary>
        /// Remove um arquivo do S3
        /// </summary>
        /// <param name="s3FileKey">Chave do arquivo no S3</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Result indicando sucesso ou erro</returns>
        Task<IResult<bool>> DeleteFileAsync(string s3FileKey, CancellationToken cancellationToken);

		/// <summary>
		/// Gera uma URL pré-assinada para acesso temporário ao arquivo
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <param name="expirationMinutes">Tempo de expiração em minutos</param>
		/// <returns>Result contendo a URL pré-assinada ou erro</returns>
		Task<IResult<string>> GeneratePresignedUrlAsync(string fileKey, int expirationMinutes = 60);
	}
}