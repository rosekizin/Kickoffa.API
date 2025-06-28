using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Wrappers;

/// <summary>
/// Wrapper para UserManager para facilitar testes unitários
/// </summary>
public class UserManagerWrapper : IUserManagerWrapper
{
    private readonly UserManager<User> _userManager;

    public UserManagerWrapper(UserManager<User> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    /// <inheritdoc />
    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<User?> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> CreateAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> UpdateAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> AddToRoleAsync(User user, string role)
    {
        return await _userManager.AddToRoleAsync(user, role);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
    {
        return await _userManager.RemoveFromRoleAsync(user, role);
    }

    /// <inheritdoc />
    public async Task<bool> IsInRoleAsync(User user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }

    /// <inheritdoc />
    public async Task<IList<string>> GetRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> SetEmailAsync(User user, string email)
    {
        return await _userManager.SetEmailAsync(user, email);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> SetUserNameAsync(User user, string userName)
    {
        return await _userManager.SetUserNameAsync(user, userName);
    }

    /// <inheritdoc />
    public async Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail)
    {
        return await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string token)
    {
        return await _userManager.ChangeEmailAsync(user, newEmail, token);
    }
}
