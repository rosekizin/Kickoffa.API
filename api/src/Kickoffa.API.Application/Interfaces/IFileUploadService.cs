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
		/// <returns>URL da imagem no S3</returns>
		Task<string> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

		/// <summary>
		/// Faz upload de um arquivo genérico para o S3 e retorna a URL
		/// </summary>
		/// <param name="file">Arquivo</param>
		/// <param name="folder">Pasta no S3 onde salvar o arquivo</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>URL do arquivo no S3</returns>
		Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

		/// <summary>
		/// Remove um arquivo do S3
		/// </summary>
		/// <param name="fileUrl">URL do arquivo no S3</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>True se removido com sucesso</returns>
		Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gera uma URL pré-assinada para acesso temporário ao arquivo
		/// </summary>
		/// <param name="fileKey">Chave do arquivo no S3</param>
		/// <param name="expirationMinutes">Tempo de expiração em minutos</param>
		/// <returns>URL pré-assinada</returns>
		Task<string> GeneratePresignedUrlAsync(string fileKey, int expirationMinutes = 60);
	}
}
