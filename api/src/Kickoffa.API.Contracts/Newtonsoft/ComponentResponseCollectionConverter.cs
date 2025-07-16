using Kickoffa.API.Contracts.Checklist.Components.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
	/// <summary>
	/// Conversor personalizado para serialização de coleções de ComponentResponse com herança
	/// </summary>
	public class ComponentResponseCollectionConverter : JsonConverter<ICollection<ComponentResponse>>
	{
		public override ICollection<ComponentResponse>? ReadJson(
			JsonReader reader,
			Type objectType,
			ICollection<ComponentResponse>? existingValue,
			bool hasExistingValue,
			JsonSerializer serializer)
		{
			var array = JArray.Load(reader);
			var result = new List<ComponentResponse>();
			var componentConverter = new ComponentResponseConverter();

			foreach (var item in array)
			{
				using var itemReader = item.CreateReader();
				var component = componentConverter.ReadJson(itemReader, typeof(ComponentResponse), null, false, serializer);
				if (component != null)
				{
					result.Add(component);
				}
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, ICollection<ComponentResponse>? value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			var componentConverter = new ComponentResponseConverter();

			writer.WriteStartArray();

			foreach (var component in value)
			{
				componentConverter.WriteJson(writer, component, serializer);
			}

			writer.WriteEndArray();
		}
	}
}
