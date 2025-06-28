namespace Kickoffa.API.Contracts.Customer;

/// <summary>
/// Response da criação de um customer
/// </summary>
public sealed record CustomerResponse
{
    public required long Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Email { get; init; }
    public string? Cpf { get; init; }
    public string? Cnpj { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public required DateTime CreatedDateUtc { get; init; }
    public required DateTime LastUpdatedDateUtc { get; init; }
}
