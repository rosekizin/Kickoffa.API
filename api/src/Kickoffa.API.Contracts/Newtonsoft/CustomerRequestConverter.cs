using Kickoffa.API.Contracts.Customer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kickoffa.API.Contracts.Newtonsoft
{
	/// <summary>
	/// Converter personalizado para deserialização de CreateCustomerRequest
	/// Suporta a hierarquia de Customer (NaturalPerson e LegalPerson)
	/// </summary>
	public class CustomerRequestConverter : JsonConverter
	{
		public override bool CanWrite => false;
		public override bool CanRead => true;

		public override bool CanConvert(Type objectType)
		{
			// Só converter se for exatamente CreateCustomerRequest, não as classes derivadas
			// para evitar loop infinito
			return objectType == typeof(CreateCustomerRequest);
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			var json = JObject.Load(reader);

			var typeStr = json["type"]?.ToString();

			if (string.IsNullOrWhiteSpace(typeStr))
				throw new JsonSerializationException("Campo 'type' obrigatório para desserialização de CreateCustomerRequest.");

			var type = Enum.Parse<CustomerType>(typeStr);
			using var subReader = json.CreateReader();

			return type switch
			{
				CustomerType.NaturalPerson => serializer.Deserialize<CreateNaturalPersonRequest>(subReader)!,
				CustomerType.LegalCompany => serializer.Deserialize<CreateLegalPersonRequest>(subReader)!,
				_ => throw new JsonSerializationException($"Tipo de customer '{typeStr}' não suportado.")
			};
		}

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value, value?.GetType() ?? typeof(CreateCustomerRequest));
		}
	}
}