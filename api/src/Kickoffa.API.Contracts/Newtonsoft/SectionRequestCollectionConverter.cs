using Kickoffa.API.Contracts.Checklist.Sections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
    /// <summary>
    /// Conversor personalizado para serialização de coleções de SectionRequest com herança
    /// </summary>
    public class SectionRequestCollectionConverter : JsonConverter<ICollection<SectionRequest>>
    {
        public override ICollection<SectionRequest>? ReadJson(
            JsonReader reader, 
            Type objectType,
            ICollection<SectionRequest>? existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            var result = new List<SectionRequest>();
            var sectionConverter = new SectionRequestConverter();

            foreach (var item in array)
            {
                using var itemReader = item.CreateReader();
                var section = sectionConverter.ReadJson(itemReader, typeof(SectionRequest), null, false, serializer);
                if (section != null)
                {
                    result.Add(section);
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, ICollection<SectionRequest>? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var sectionConverter = new SectionRequestConverter();
            
            writer.WriteStartArray();
            
            foreach (var section in value)
            {
                sectionConverter.WriteJson(writer, section, serializer);
            }
            
            writer.WriteEndArray();
        }
    }
}
