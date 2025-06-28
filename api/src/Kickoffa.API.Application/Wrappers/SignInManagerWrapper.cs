using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Wrappers;

/// <summary>
/// Wrapper para SignInManager para facilitar testes unitários
/// </summary>
public class SignInManagerWrapper : ISignInManagerWrapper
{
    private readonly SignInManager<User> _signInManager;

    public SignInManagerWrapper(SignInManager<User> signInManager)
    {
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    /// <inheritdoc />
    public async Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure)
    {
        return await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }

    /// <inheritdoc />
    public async Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        return await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
    }

    /// <inheritdoc />
    public async Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
    {
        return await _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
    }

    /// <inheritdoc />
    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    /// <inheritdoc />
    public async Task<bool> CanSignInAsync(User user)
    {
        return await _signInManager.CanSignInAsync(user);
    }
}
