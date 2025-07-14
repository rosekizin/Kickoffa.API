using Kickoffa.API.Contracts.Checklist.Sections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
    /// <summary>
    /// Conversor personalizado para serialização de SectionResponse com herança
    /// </summary>
    public class SectionResponseConverter : JsonConverter<SectionResponse>
    {
        public override SectionResponse? ReadJson(
            JsonReader reader, 
            Type objectType,
            SectionResponse? existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            // Captura o campo "type" como string
            var typeStr = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(typeStr))
                throw new JsonSerializationException("Campo 'type' obrigatório para desserialização de SectionResponse.");

            using var subReader = json.CreateReader();

            return typeStr.ToLowerInvariant() switch
            {
                "briefing" => serializer.Deserialize<BriefingSectionResponse>(subReader),
                "checklist" => serializer.Deserialize<ChecklistSectionResponse>(subReader),
                _ => throw new JsonSerializationException($"Tipo de seção '{typeStr}' não suportado.")
            };
        }

        public override void WriteJson(JsonWriter writer, SectionResponse? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // Criar um JObject para manipular a serialização
            var jObject = new JObject();

            // Serializar todas as propriedades do objeto específico
            var specificType = value.GetType();
            var tempJson = JsonConvert.SerializeObject(value, specificType, null);
            var tempObject = JObject.Parse(tempJson);

            // Copiar todas as propriedades
            foreach (var property in tempObject.Properties())
            {
                jObject[property.Name] = property.Value;
            }

            // Garantir que o campo "type" está correto baseado no tipo específico
            jObject["type"] = value switch
            {
                BriefingSectionResponse => "briefing",
                ChecklistSectionResponse => "checklist",
                _ => value.Type // fallback para o valor original
            };

            // Escrever o JSON final
            jObject.WriteTo(writer);
        }
    }
}