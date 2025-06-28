using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Application.Services.AppUser;

/// <summary>
/// Serviço para inicialização de roles padrão
/// </summary>
public class RoleInitializationService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleInitializationService(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    /// <summary>
    /// Inicializa as roles padrão do sistema
    /// </summary>
    public async Task InitializeDefaultRolesAsync()
    {
        var defaultRoles = new[]
        {
            "freelancer"
        };

        foreach (var roleName in defaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role(roleName);
                await _roleManager.CreateAsync(role);
            }
        }
    }
}