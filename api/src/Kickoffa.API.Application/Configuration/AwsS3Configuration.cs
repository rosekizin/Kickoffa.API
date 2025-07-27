namespace Kickoffa.API.Application.Configuration
{
	/// <summary>
	/// Configurações para AWS S3 e proxy de imagens
	/// </summary>
	public class AwsS3Configuration
	{
		/// <summary>
		/// Nome do bucket S3
		/// </summary>
		public string BucketName { get; set; } = string.Empty;

		/// <summary>
		/// Região do AWS S3
		/// </summary>
		public string Region { get; set; } = string.Empty;

		/// <summary>
		/// Chave de acesso AWS
		/// </summary>
		public string AccessKey { get; set; } = string.Empty;

		/// <summary>
		/// Chave secreta AWS
		/// </summary>
		public string SecretKey { get; set; } = string.Empty;

		/// <summary>
		/// URL base do bucket
		/// </summary>
		public string BaseUrl { get; set; } = string.Empty;

		/// <summary>
		/// Indica se deve usar LocalStack para desenvolvimento
		/// </summary>
		public bool UseLocalStack { get; set; } = false;

		/// <summary>
		/// URL do LocalStack
		/// </summary>
		public string LocalStackUrl { get; set; } = string.Empty;

		/// <summary>
		/// URL base da API para gerar URLs do proxy
		/// </summary>
		public string ApiBaseUrl { get; set; } = "http://localhost:5084";
	}
}
