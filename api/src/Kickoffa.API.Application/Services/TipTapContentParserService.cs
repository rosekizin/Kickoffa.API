using Kickoffa.API.Application.Interfaces;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Application.Services
{
	/// <summary>
	/// Serviço para parsing de conteúdo TipTap
	/// </summary>
	public class TipTapContentParserService : ITipTapContentParserService
	{
        /// <summary>
        /// Extrai URLs de imagens do contentJson do TipTap
        /// </summary>
        /// <param name="contentJson">JSON do conteúdo do TipTap</param>
        /// <returns>Lista de URLs de imagens encontradas</returns>
        public List<string> ExtractImageUrlsFromContentJson(string contentJson)
        {
            var imageUrls = new List<string>();

            try
            {
                var root = JToken.Parse(contentJson);
                Traverse(root, imageUrls);
            }
            catch
            {
                // JSON inválido → retorna vazio
            }

            return imageUrls;
        }

        private static void Traverse(JToken token, List<string> imageUrls)
        {
            if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;

                // Se for um node de imagem, extrai o src
                if (obj["type"]?.ToString() == "image")
                {
                    var src = obj["attrs"]?["src"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(src))
                        imageUrls.Add(src);
                }

                // Percorre as propriedades do objeto
                foreach (var property in obj.Properties())
                {
                    Traverse(property.Value, imageUrls);
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in token.Children())
                {
                    Traverse(item, imageUrls);
                }
            }
        }

        /// <summary>
        /// Extrai o nome do arquivo de uma URL
        /// </summary>
        /// <param name="url">URL da imagem</param>
        /// <returns>Nome do arquivo extraído da URL</returns>
        public string ExtractFileNameFromUrl(string url)
		{
			try
			{
				var uri = new Uri(url);
				var fileName = Path.GetFileName(uri.LocalPath);
				return string.IsNullOrWhiteSpace(fileName) ? "image.jpg" : fileName;
			}
			catch
			{
				return "image.jpg";
			}
		}

		/// <summary>
		/// Compara duas listas de URLs e retorna as diferenças
		/// </summary>
		/// <param name="currentUrls">URLs atuais (do banco)</param>
		/// <param name="newUrls">URLs novas (do request)</param>
		/// <returns>Tupla com URLs para adicionar e URLs para remover</returns>
		public (List<string> toAdd, List<string> toRemove) CompareImageUrls(
			IEnumerable<string> currentUrls, 
			IEnumerable<string> newUrls)
		{
			var currentSet = currentUrls.ToHashSet();
			var newSet = newUrls.ToHashSet();

			var toAdd = newSet.Except(currentSet).ToList();
			var toRemove = currentSet.Except(newSet).ToList();

			return (toAdd, toRemove);
		}
	}
}