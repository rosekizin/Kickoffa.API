namespace Kickoffa.API.Contracts.FileUpload
{
	/// <summary>
	/// Resposta do upload de arquivo
	/// </summary>
	public class FileUploadResponse
	{
		/// <summary>
		/// URL do arquivo (proxy URL para acesso seguro)
		/// </summary>
		public string Url { get; set; } = string.Empty;

		/// <summary>
		/// Nome original do arquivo
		/// </summary>
		public string FileName { get; set; } = string.Empty;

		/// <summary>
		/// Tipo de conteúdo do arquivo
		/// </summary>
		public string ContentType { get; set; } = string.Empty;

		/// <summary>
		/// Tamanho do arquivo em bytes
		/// </summary>
		public long Size { get; set; }
	}
}
