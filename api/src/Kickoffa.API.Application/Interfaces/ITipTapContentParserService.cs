namespace Kickoffa.API.Application.Interfaces
{
    /// <summary>
    /// Interface para serviço de parsing de conteúdo TipTap
    /// </summary>
    public interface ITipTapContentParserService
    {
        /// <summary>
        /// Extrai URLs de imagens do contentJson do TipTap
        /// </summary>
        /// <param name="contentJson">JSON do conteúdo do TipTap</param>
        /// <returns>Lista de URLs de imagens encontradas</returns>
        List<string> ExtractImageUrlsFromContentJson(string contentJson);

        /// <summary>
        /// Extrai o nome do arquivo de uma URL
        /// </summary>
        /// <param name="url">URL da imagem</param>
        /// <returns>Nome do arquivo extraído da URL</returns>
        string ExtractFileNameFromUrl(string url);

        /// <summary>
        /// Compara duas listas de URLs e retorna as diferenças
        /// </summary>
        /// <param name="currentUrls">URLs atuais (do banco)</param>
        /// <param name="newUrls">URLs novas (do request)</param>
        /// <returns>Tupla com URLs para adicionar e URLs para remover</returns>
        (List<string> toAdd, List<string> toRemove) CompareImageUrls(
            IEnumerable<string> currentUrls,
            IEnumerable<string> newUrls);
    }
}