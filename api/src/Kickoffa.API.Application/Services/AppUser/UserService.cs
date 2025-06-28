using Kickoffa.API.Domain.Models.AppUser;
using Kickoffa.API.Domain.Repositories;

namespace Kickoffa.API.Application.Services.AppUser;

/// <summary>
/// Serviço para operações relacionadas a User
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Inicializa uma nova instância do UserService
    /// </summary>
    /// <param name="userRepository">Repositório de users</param>
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        return await _userRepository.GetByEmailAndPasswordAsync(email, password, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _userRepository.EmailExistsAsync(email, cancellationToken);
    }
}
