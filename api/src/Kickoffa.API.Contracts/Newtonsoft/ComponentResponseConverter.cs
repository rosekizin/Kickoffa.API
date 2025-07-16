using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Contracts.Checklist.Components.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
    /// <summary>
    /// Conversor personalizado para serialização de ComponentResponse com herança
    /// </summary>
    public class ComponentResponseConverter : JsonConverter<ComponentResponse>
    {
        public override ComponentResponse? ReadJson(
            JsonReader reader, 
            Type objectType,
            ComponentResponse? existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            // Captura o campo "type" como string
            var typeStr = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(typeStr))
                throw new JsonSerializationException("Campo 'type' obrigatório para desserialização de ComponentResponse.");

            using var subReader = json.CreateReader();

            return typeStr.ToLowerInvariant() switch
            {
                "text" => serializer.Deserialize<TextComponentResponse>(subReader),
                "upload" => serializer.Deserialize<UploadComponentResponse>(subReader),
                "checkbox" => serializer.Deserialize<CheckboxComponentResponse>(subReader),
                "signature" => serializer.Deserialize<SignatureComponentResponse>(subReader),
                "confirmation" => serializer.Deserialize<ConfirmationComponentResponse>(subReader),
                _ => throw new JsonSerializationException($"Tipo de componente '{typeStr}' não suportado.")
            };
        }

        public override void WriteJson(JsonWriter writer, ComponentResponse? value, JsonSerializer serializer)
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
            //var tempJson = JsonConvert.SerializeObject(value, specificType, serializer.Settings);
            var tempObject = JObject.Parse(tempJson);

            // Copiar todas as propriedades
            foreach (var property in tempObject.Properties())
            {
                jObject[property.Name] = property.Value;
            }

            // Garantir que o campo "type" está correto baseado no tipo específico
            jObject["type"] = value switch
            {
                CheckboxComponentResponse => "checkbox",
                TextComponentResponse => "text",
                UploadComponentResponse => "upload",
                SignatureComponentResponse => "signature",
                ConfirmationComponentResponse => "confirmation",
                _ => value.Type // fallback para o valor original
            };

            // Escrever o JSON final
            jObject.WriteTo(writer);
        }
    }
}
