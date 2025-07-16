using Kickoffa.API.Contracts.Checklist.Components.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
	public class ComponentRequestCollectionConverter : JsonConverter<ICollection<ComponentRequest>>
	{
		private readonly ComponentRequestConverter _itemConverter = new();

		public override ICollection<ComponentRequest>? ReadJson(
			JsonReader reader,
			Type objectType,
			ICollection<ComponentRequest>? existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			var array = JArray.Load(reader);
			var list = new List<ComponentRequest>();

			foreach (var item in array)
			{
				using var subReader = item.CreateReader();
				var component = _itemConverter.ReadJson(subReader, typeof(ComponentRequest), null, false, serializer);
				list.Add(component!);
			}

			return list;
		}

		public override void WriteJson(JsonWriter writer, ICollection<ComponentRequest>? value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}
	}
}