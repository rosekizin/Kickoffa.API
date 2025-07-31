using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Contracts.Checklist;
using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Domain.Interfaces.Models;
using Kickoffa.API.Domain.Models;
using Kickoffa.API.Domain.Models.Enums;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Application.Factories
{
	///<inheritdoc/>
	public class SectionFactory : ISectionFactory
	{
		private readonly IComponentFactory _componentFactory;
		private readonly IBriefingMediaFactory _briefingMediaFactory;

		public SectionFactory(
			IComponentFactory componentFactory,
			IBriefingMediaFactory briefingMediaFactory)
		{
			_componentFactory = componentFactory;
			_briefingMediaFactory = briefingMediaFactory;
		}

		///<inheritdoc/>
		public async Task<ISection> CreateSection(SectionRequest sectionRequest, CancellationToken cancellationToken)
		{
			return sectionRequest.Type switch
			{
				SectionTypeRequest.Briefing => CreateBriefingSection(0, sectionRequest),
				SectionTypeRequest.Checklist => await CreateChecklistSection(sectionRequest, cancellationToken),
				_ => throw new ArgumentException($"Tipo de se��o inv�lido: {sectionRequest.Type}")
			};
		}

		///<inheritdoc/>
		public IBriefingSection CreateBriefingSection(
			long checklistId,
			SectionRequest sectionRequest)
		{
			var briefingSection = ((BriefingSectionRequest)sectionRequest);
			var section = new BriefingSection(checklistId, briefingSection.Title, briefingSection.Order, briefingSection.ContentJson, briefingSection.ContentHtml);

			// Identificar e processar imagens no contentJson
			if (!string.IsNullOrWhiteSpace(briefingSection.ContentJson))
			{
				var imageUrls = ExtractImageUrlsFromContentJson(briefingSection.ContentJson);
				ProcessImageUrls(section, imageUrls);
			}

			return section;
		}

		///<inheritdoc/>
		public IBriefingSection CreateBriefingSection(
			long checklistId,
			string title,
			int order,
			string? contentJson = null,
			string? contentHtml = null)
		{
			return new BriefingSection(checklistId, title, order, contentJson, contentHtml);
		}

		///<inheritdoc/>
		public IChecklistSection CreateChecklistSection(long checklistId, string title, int order)
		{
			return new ChecklistSection(checklistId, title, order);
		}

		///<inheritdoc/>
		public bool IsValidSectionType(SectionType type)
		{
			return Enum.IsDefined(type);
		}

		///<inheritdoc/>
		public SectionType[] GetAvailableSectionTypes()
		{
			return Enum.GetValues<SectionType>();
		}

        /// <summary>
        /// Extrai URLs de imagens do contentJson do TipTap
        /// </summary>
        /// <param name="contentJson">JSON do conteúdo do TipTap</param>
        /// <returns>Lista de URLs de imagens encontradas</returns>
        private List<string> ExtractImageUrlsFromContentJson(string contentJson)
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

		Tem que implementar isso no update de checklist
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
        /// Processa as URLs de imagens e adiciona como media na seção
        /// </summary>
        /// <param name="section">Seção de briefing</param>
        /// <param name="imageUrls">Lista de URLs de imagens</param>
        private void ProcessImageUrls(IBriefingSection section, List<string> imageUrls)
		{
			foreach (var imageUrl in imageUrls)
			{
				try
				{
					// Extrair informações da URL
					var fileName = ExtractFileNameFromUrl(imageUrl);

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
					continue;
				}
			}
		}

        public static string GetPathFromUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return uri.AbsolutePath;

            return string.Empty;
        }

        /// <summary>
        /// Extrai o nome do arquivo de uma URL
        /// </summary>
        /// <param name="url">URL da imagem</param>
        /// <returns>Nome do arquivo extraído da URL</returns>
        private string ExtractFileNameFromUrl(string url)
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
		/// Cria uma se��o de checklist com seus componentes
		/// </summary>
		private async Task<IChecklistSection> CreateChecklistSection(SectionRequest sectionRequest,	CancellationToken cancellationToken)
		{
			var section = CreateChecklistSection(
				checklistId: 0, // Ser� definido quando adicionado ao checklist
				title: sectionRequest.Title,
				order: sectionRequest.Order
			);

			// Adicionar componentes se existirem
			if (sectionRequest is ChecklistSectionRequest checklistSection && checklistSection.Components is not null)
			{
				foreach (var componentRequest in checklistSection.Components.OrderBy(c => c.Order))
				{
					var component = await _componentFactory.CreateComponent(componentRequest, cancellationToken);
					section.AddComponent(component);
				}
			}

			return section;
		}
	}
}