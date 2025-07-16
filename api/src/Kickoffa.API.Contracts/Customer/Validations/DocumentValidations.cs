namespace Kickoffa.API.Contracts.Customer.Validations
{
	public class DocumentValidations
	{
		/// <summary>
		/// Valida CPF usando algoritmo oficial
		/// </summary>
		/// <param name="cpf">CPF a ser validado</param>
		/// <returns>True se válido</returns>
		public static bool IsValidCpf(string cpf)
		{
			if (string.IsNullOrWhiteSpace(cpf) || AreThereNonNumericChars(cpf))
				return false;

			if (cpf.Length != 11)
				return false;

			// CPFs inválidos conhecidos
			if (cpf == "00000000000" || cpf == "11111111111" || cpf == "22222222222" ||
				cpf == "33333333333" || cpf == "44444444444" || cpf == "55555555555" ||
				cpf == "66666666666" || cpf == "77777777777" || cpf == "88888888888" ||
				cpf == "99999999999")
			{
				return false;
			}

			// Validação do primeiro dígito verificador
			var sum = 0;
			for (int i = 0; i < 9; i++)
			{
				sum += int.Parse(cpf[i].ToString()) * (10 - i);
			}
			var remainder = sum % 11;
			var digit1 = remainder < 2 ? 0 : 11 - remainder;

			if (int.Parse(cpf[9].ToString()) != digit1)
				return false;

			// Validação do segundo dígito verificador
			sum = 0;
			for (int i = 0; i < 10; i++)
			{
				sum += int.Parse(cpf[i].ToString()) * (11 - i);
			}
			remainder = sum % 11;
			var digit2 = remainder < 2 ? 0 : 11 - remainder;

			return int.Parse(cpf[10].ToString()) == digit2;
		}

		/// <summary>
		/// Valida CNPJ usando algoritmo oficial
		/// </summary>
		/// <param name="cnpj">CNPJ a ser validado</param>
		/// <returns>True se válido</returns>
		public static bool IsValidCnpj(string cnpj)
		{
			if (string.IsNullOrWhiteSpace(cnpj) || AreThereNonNumericChars(cnpj))
				return false;

			if (cnpj.Length != 14)
				return false;

			// CNPJs inválidos conhecidos
			if (cnpj == "00000000000000" || cnpj == "11111111111111" || cnpj == "22222222222222" ||
				cnpj == "33333333333333" || cnpj == "44444444444444" || cnpj == "55555555555555" ||
				cnpj == "66666666666666" || cnpj == "77777777777777" || cnpj == "88888888888888" ||
				cnpj == "99999999999999")
			{
				return false;
			}

			// Validação do primeiro dígito verificador
			var weights1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			var sum = 0;
			for (int i = 0; i < 12; i++)
			{
				sum += int.Parse(cnpj[i].ToString()) * weights1[i];
			}
			var remainder = sum % 11;
			var digit1 = remainder < 2 ? 0 : 11 - remainder;

			if (int.Parse(cnpj[12].ToString()) != digit1)
				return false;

			// Validação do segundo dígito verificador
			var weights2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			sum = 0;
			for (int i = 0; i < 13; i++)
			{
				sum += int.Parse(cnpj[i].ToString()) * weights2[i];
			}
			remainder = sum % 11;
			var digit2 = remainder < 2 ? 0 : 11 - remainder;

			return int.Parse(cnpj[13].ToString()) == digit2;
		}

		public static bool AreThereNonNumericChars(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return true;

			foreach (char c in input)
			{
				if (!char.IsDigit(c))
					return true;
			}

			return false;
		}
	}
}