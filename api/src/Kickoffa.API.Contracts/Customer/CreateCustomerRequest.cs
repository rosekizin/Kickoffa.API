using Kickoffa.API.Contracts.Customer.Validations;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Kickoffa.API.Contracts.Customer;

/// <summary>
/// Request para criação de um novo customer
/// </summary>
public sealed record CreateCustomerRequest : IValidatableObject
{
	public required string FirstName { get; init; }
	public required string LastName { get; init; }
	public string? Email { get; init; }
	public string? Cpf { get; init; }
	public string? Cnpj { get; init; }
	public string? PhoneNumber { get; init; }
	public string? Address { get; init; }

	/// <summary>
	/// Valida as propriedades do request
	/// </summary>
	/// <param name="validationContext">Contexto de validação</param>
	/// <returns>Resultados da validação</returns>
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var results = new List<ValidationResult>();

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

		// Validação do CPF (opcional)
		if (!string.IsNullOrWhiteSpace(Cpf))
		{
			if (Cpf.Length != 11)
			{
				results.Add(new ValidationResult("O CPF deve ter exatamente 11 dígitos", [nameof(Cpf)]));
			}
			else if (!Regex.IsMatch(Cpf, @"^\d{11}$"))
			{
				results.Add(new ValidationResult("O CPF deve conter apenas números", [nameof(Cpf)]));
			}
			else if (!DocumentValidations.IsValidCpf(Cpf))
			{
				results.Add(new ValidationResult("CPF inválido", [nameof(Cpf)]));
			}
		}

		// Validação do CNPJ (opcional)
		if (!string.IsNullOrWhiteSpace(Cnpj))
		{
			if (Cnpj.Length != 14)
			{
				results.Add(new ValidationResult("O CNPJ deve ter exatamente 14 dígitos", [nameof(Cnpj)]));
			}
			else if (!Regex.IsMatch(Cnpj, @"^\d{14}$"))
			{
				results.Add(new ValidationResult("O CNPJ deve conter apenas números", [nameof(Cnpj)]));
			}
			else if (!DocumentValidations.IsValidCnpj(Cnpj))
			{
				results.Add(new ValidationResult("CNPJ inválido", [nameof(Cnpj)]));
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

		// Validação de regra de negócio: deve ter CPF ou CNPJ
		if (string.IsNullOrWhiteSpace(Cpf) && string.IsNullOrWhiteSpace(Cnpj))
		{
			results.Add(new ValidationResult("É obrigatório informar CPF ou CNPJ", [nameof(Cpf), nameof(Cnpj)]));
		}

		// Validação de regra de negócio: não pode ter CPF e CNPJ ao mesmo tempo
		if (!string.IsNullOrWhiteSpace(Cpf) && !string.IsNullOrWhiteSpace(Cnpj))
		{
			results.Add(new ValidationResult("Não é possível informar CPF e CNPJ simultaneamente", [nameof(Cpf), nameof(Cnpj)]));
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