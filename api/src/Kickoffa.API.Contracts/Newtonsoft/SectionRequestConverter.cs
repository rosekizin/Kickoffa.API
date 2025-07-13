using Kickoffa.API.Contracts.Checklist.Sections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
    /// <summary>
    /// Conversor personalizado para deserialização de SectionRequest com herança
    /// </summary>
    public class SectionRequestConverter : JsonConverter<SectionRequest>
    {
        public override SectionRequest? ReadJson(
            JsonReader reader, 
            Type objectType,
            SectionRequest? existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            // Captura o campo "type" como string
            var typeStr = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(typeStr))
                throw new JsonSerializationException("Campo 'type' obrigatório para desserialização de SectionRequest.");

            using var subReader = json.CreateReader();

            return typeStr.ToLowerInvariant() switch
            {
                "briefing" => serializer.Deserialize<BriefingSectionRequest>(subReader),
                "checklist" => serializer.Deserialize<ChecklistSectionRequest>(subReader),
                _ => throw new JsonSerializationException($"Tipo de seção '{typeStr}' não suportado.")
            };
        }

        public override void WriteJson(JsonWriter writer, SectionRequest? value, JsonSerializer serializer)
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
                BriefingSectionRequest => "briefing",
                ChecklistSectionRequest => "checklist",
                _ => value.Type.ToString() // fallback para o valor original
            };

            // Escrever o JSON final
            jObject.WriteTo(writer);
        }
    }
}