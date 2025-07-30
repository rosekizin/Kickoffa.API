namespace Kickoffa.API.Contracts.FileUpload
{
	/// <summary>
	/// Resposta da geração de URL pré-assinada
	/// </summary>
	public class PresignedUrlResponse
	{
		/// <summary>
		/// URL pré-assinada para acesso temporário ao arquivo
		/// </summary>
		public string Url { get; set; } = string.Empty;

		/// <summary>
		/// Data e hora de expiração da URL
		/// </summary>
		public DateTime ExpiresAt { get; set; }

		/// <summary>
		/// Chave do arquivo no S3
		/// </summary>
		public string FileKey { get; set; } = string.Empty;
	}
}
