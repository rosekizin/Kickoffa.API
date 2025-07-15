using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Customer;

/// <summary>
/// Response base abstrata para Customer usando hierarquia
/// </summary>
public sealed record CustomerResponse
{
	[JsonProperty(PropertyName = "id", Required = Required.Always)]
	public required long Id { get; init; }

	[JsonProperty(PropertyName = "phoneNumber", Required = Required.Default)]
	public string? PhoneNumber { get; init; }

	[JsonProperty(PropertyName = "address", Required = Required.Default)]
	public string? Address { get; init; }

	[JsonProperty(PropertyName = "email", Required = Required.Default)]
	public string? Email { get; init; }

	[JsonProperty(PropertyName = "type", Required = Required.Always)]
	public required CustomerType Type { get; init; }

	[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
	public required DateTime CreatedDateUtc { get; init; }

	[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
	public required DateTime LastUpdatedDateUtc { get; init; }

	// NaturalPerson properties

	[JsonProperty(PropertyName = "firstName", Required = Required.Default)]
	public string? FirstName { get; init; }

	[JsonProperty(PropertyName = "lastName", Required = Required.Default)]
	public string? LastName { get; init; }

	[JsonProperty(PropertyName = "cpf", Required = Required.Default)]
	public string? Cpf { get; init; }

	//  LegalPerson properties

	[JsonProperty(PropertyName = "company", Required = Required.Default)]
	public string? Company { get; init; } // Razão Social

	[JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
	public string? Cnpj { get; init; }
}