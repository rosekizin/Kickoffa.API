using Kickoffa.API.AspNet.Infrastructure.Configuration.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.ServiceCollection;

/// <summary>
/// Extensões para configuração de autenticação
/// </summary>
public static class AuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona e configura autenticação JWT
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="jwtConfiguration">Configuração JWT</param>
    /// <returns>Coleção de serviços para chaining</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        JwtConfiguration jwtConfiguration)
    {
        // Registrar a configuração como singleton
        services.AddSingleton(jwtConfiguration);

        // Configurar autenticação
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Para desenvolvimento
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Validar o emissor
                ValidateIssuer = true,
                ValidIssuer = jwtConfiguration.Issuer,

                // Validar a audiência
                ValidateAudience = true,
                ValidAudience = jwtConfiguration.Audience,

                // Validar a chave de assinatura
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey)),

                // Validar tempo de vida
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // Remove tolerância de tempo padrão (5 minutos)

                // Configurações adicionais de segurança
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            // Eventos para logging e debugging
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    // Primeiro, tentar obter token do header Authorization
                    var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

                    // Se não encontrar no header, tentar obter do cookie HttpOnly
                    if (string.IsNullOrEmpty(token))
                    {
                        token = context.Request.Cookies["access_token"];
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    // Log do erro de autenticação
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<JwtBearerEvents>>();
                    
                    logger.LogWarning("Falha na autenticação JWT: {Error}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Log de token validado com sucesso
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<JwtBearerEvents>>();
                    
                    var userId = context.Principal?.Identity?.Name;
                    logger.LogInformation("Token JWT validado com sucesso para usuário: {UserId}", userId);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    // Log de desafio de autenticação
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<JwtBearerEvents>>();
                    
                    logger.LogWarning("Desafio de autenticação JWT: {Error}", context.Error);
                    return Task.CompletedTask;
                }
            };
        });

        // Configurar autorização
        services.AddAuthorization(options =>
        {
            // Política padrão que requer autenticação
            options.FallbackPolicy = options.DefaultPolicy;
            
            // Políticas personalizadas podem ser adicionadas aqui
            // Exemplo:
            // options.AddPolicy("AdminOnly", policy => 
            //     policy.RequireClaim("role", "admin"));
        });

        return services;
    }
}