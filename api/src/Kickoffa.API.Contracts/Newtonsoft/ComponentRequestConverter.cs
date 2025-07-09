using Kickoffa.API.Contracts.Checklist.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
	public class ComponentRequestConverter : JsonConverter<ComponentRequest>
	{
		public override ComponentRequest? ReadJson(
			JsonReader reader, 
			Type objectType,
			ComponentRequest? existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			var json = JObject.Load(reader);

			// Captura o campo "type" como string
			var typeStr = json["type"]?.ToString();

			if (string.IsNullOrWhiteSpace(typeStr))
				throw new JsonSerializationException("Campo 'type' obrigatório para desserialização de ComponentRequest.");

			using var subReader = json.CreateReader();

			return typeStr.ToLowerInvariant() switch
			{
				"text" => serializer.Deserialize<TextComponentRequest>(subReader),
				"upload" => serializer.Deserialize<UploadComponentRequest>(subReader),
				"checkbox" => serializer.Deserialize<CheckboxComponentRequest>(subReader),
				"signature" => serializer.Deserialize<SignatureComponentRequest>(subReader),
				"confirmation" => serializer.Deserialize<ConfirmationComponentRequest>(subReader),
				_ => throw new JsonSerializationException($"Tipo de componente '{typeStr}' não suportado.")
			};
		}

		public override void WriteJson(JsonWriter writer, ComponentRequest? value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value, value?.GetType() ?? typeof(ComponentRequest));
		}
	}
}