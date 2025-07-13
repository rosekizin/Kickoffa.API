using Kickoffa.API.Contracts.Checklist.Sections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
    /// <summary>
    /// Conversor personalizado para serialização de coleções de SectionResponse com herança
    /// </summary>
    public class SectionResponseCollectionConverter : JsonConverter<ICollection<SectionResponse>>
    {
        public override ICollection<SectionResponse>? ReadJson(
            JsonReader reader, 
            Type objectType,
            ICollection<SectionResponse>? existingValue,
            bool hasExistingValue, 
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            var result = new List<SectionResponse>();
            var sectionConverter = new SectionResponseConverter();

            foreach (var item in array)
            {
                using var itemReader = item.CreateReader();
                var section = sectionConverter.ReadJson(itemReader, typeof(SectionResponse), null, false, serializer);
                if (section != null)
                {
                    result.Add(section);
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, ICollection<SectionResponse>? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var sectionConverter = new SectionResponseConverter();
            
            writer.WriteStartArray();
            
            foreach (var section in value)
            {
                sectionConverter.WriteJson(writer, section, serializer);
            }
            
            writer.WriteEndArray();
        }
    }
}
