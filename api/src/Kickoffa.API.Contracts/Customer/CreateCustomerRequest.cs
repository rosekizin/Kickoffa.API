using Kickoffa.API.Contracts.Customer.Validations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Kickoffa.API.Contracts.Customer;

/// <summary>
/// Request base abstrato para criação de um novo customer
/// </summary>
///
public abstract record CreateCustomerRequest : IValidatableObject
{
	[JsonProperty(PropertyName = "id", Required = Required.Default)]
	public long Id { get; init; }

	[JsonProperty(PropertyName = "email", Required = Required.Default)]
	public string? Email { get; init; }

	[JsonProperty(PropertyName = "phoneNumber", Required = Required.Default)]
	public string? PhoneNumber { get; init; }

	[JsonProperty(PropertyName = "address", Required = Required.Default)]
	public string? Address { get; init; }

	[JsonProperty(PropertyName = "type", Required = Required.Always)]
	public CustomerType Type { get; init; }

	/// <summary>
	/// Valida as propriedades do request
	/// </summary>
	/// <param name="validationContext">Contexto de validação</param>
	/// <returns>Resultados da validação</returns>
	public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var results = new List<ValidationResult>();

		// Validação do Email (opcional)
		if (!string.IsNullOrWhiteSpace(Email))
		{
			if (Email.Length > 255)
			{
				results.Add(new ValidationResult("O email deve ter no máximo 255 caracteres", [nameof(Email)]));
			}
			else if (!IsValidEmail(Email))
			{
				results.Add(new ValidationResult("Email deve ter um formato válido", [nameof(Email)]));
			}
		}

		// Validação do PhoneNumber (opcional)
		if (!string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber.Length > 20)
		{
			results.Add(new ValidationResult("O telefone deve ter no máximo 20 caracteres", [nameof(PhoneNumber)]));
		}

		// Validação do Address (opcional)
		if (!string.IsNullOrWhiteSpace(Address) && Address.Length > 500)
		{
			results.Add(new ValidationResult("O endereço deve ter no máximo 500 caracteres", [nameof(Address)]));
		}

		return results;
	}

	/// <summary>
	/// Valida se o email tem formato válido
	/// </summary>
	/// <param name="email">Email a ser validado</param>
	/// <returns>True se válido</returns>
	private static bool IsValidEmail(string email)
	{
		try
		{
			var addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == email;
		}
		catch
		{
			return false;
		}
	}
}

/// <summary>
/// Request para criação de pessoa física (NaturalPerson)
/// </summary>
public sealed partial record CreateNaturalPersonRequest : CreateCustomerRequest
{
	[JsonProperty(PropertyName = "firstName", Required = Required.Always)]
	public required string FirstName { get; init; }

	[JsonProperty(PropertyName = "lastName", Required = Required.Always)]
	public required string LastName { get; init; }

	[JsonProperty(PropertyName = "cpf", Required = Required.Always)]
	public string? Cpf { get; init; }

	public CreateNaturalPersonRequest()
	{
		Type = CustomerType.NaturalPerson;
	}

	public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var results = base.Validate(validationContext).ToList();

		// Validação do FirstName
		if (string.IsNullOrWhiteSpace(FirstName))
		{
			results.Add(new ValidationResult("O nome é obrigatório", [nameof(FirstName)]));
		}
		else if (FirstName.Length < 2 || FirstName.Length > 100)
		{
			results.Add(new ValidationResult("O nome deve ter entre 2 e 100 caracteres", [nameof(FirstName)]));
		}

		// Validação do LastName
		if (string.IsNullOrWhiteSpace(LastName))
		{
			results.Add(new ValidationResult("O sobrenome é obrigatório", [nameof(LastName)]));
		}
		else if (LastName.Length < 2 || LastName.Length > 100)
		{
			results.Add(new ValidationResult("O sobrenome deve ter entre 2 e 100 caracteres", [nameof(LastName)]));
		}

		// Validação do CPF (opcional)
		if (!string.IsNullOrWhiteSpace(Cpf))
		{
			if (Cpf.Length != 11)
			{
				results.Add(new ValidationResult("O CPF deve ter exatamente 11 dígitos", [nameof(Cpf)]));
			}
			else if (!CpfLength().IsMatch(Cpf))
			{
				results.Add(new ValidationResult("O CPF deve conter apenas números", [nameof(Cpf)]));
			}
			else if (!DocumentValidations.IsValidCpf(Cpf))
			{
				results.Add(new ValidationResult("CPF inválido", [nameof(Cpf)]));
			}
		}

		return results;
	}

	[GeneratedRegex(@"^\d{11}$")]
	private static partial Regex CpfLength();
}

/// <summary>
/// Request para criação de pessoa jurídica (LegalPerson)
/// </summary>
public sealed partial record CreateLegalPersonRequest : CreateCustomerRequest
{
	[JsonProperty(PropertyName = "company", Required = Required.Default)]
	public required string Company { get; init; } // Razão Social

	[JsonProperty(PropertyName = "cnpj", Required = Required.Default)]
	public string? Cnpj { get; init; }

	public CreateLegalPersonRequest()
	{
		Type = CustomerType.LegalCompany;
	}

	public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var results = base.Validate(validationContext).ToList();

		// Validação do Company
		if (string.IsNullOrWhiteSpace(Company))
		{
			results.Add(new ValidationResult("O nome da empresa é obrigatório", [nameof(Company)]));
		}
		else if (Company.Length < 2 || Company.Length > 200)
		{
			results.Add(new ValidationResult("O nome da empresa deve ter entre 2 e 200 caracteres", [nameof(Company)]));
		}

		// Validação do CNPJ (opcional)
		if (!string.IsNullOrWhiteSpace(Cnpj))
		{
			if (Cnpj.Length != 14)
			{
				results.Add(new ValidationResult("O CNPJ deve ter exatamente 14 dígitos", [nameof(Cnpj)]));
			}
			else if (!CNPJLength().IsMatch(Cnpj))
			{
				results.Add(new ValidationResult("O CNPJ deve conter apenas números", [nameof(Cnpj)]));
			}
			else if (!DocumentValidations.IsValidCnpj(Cnpj))
			{
				results.Add(new ValidationResult("CNPJ inválido", [nameof(Cnpj)]));
			}
		}

		return results;
	}

	[GeneratedRegex(@"^\d{14}$")]
	private static partial Regex CNPJLength();
}