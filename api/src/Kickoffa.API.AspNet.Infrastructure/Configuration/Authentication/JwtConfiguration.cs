using Kickoffa.API.AspNet.Infrastructure.Wrappers;

namespace Kickoffa.API.AspNet.Infrastructure.Configuration.Authentication;

/// <summary>
/// Configuração para autenticação JWT
/// </summary>
public sealed class JwtConfiguration
{
    /// <summary>
    /// Chave secreta para assinatura do token
    /// </summary>
    public string SecretKey { get; }

    /// <summary>
    /// Emissor do token (issuer)
    /// </summary>
    public string Issuer { get; }

    /// <summary>
    /// Audiência do token (audience)
    /// </summary>
    public string Audience { get; }

    /// <summary>
    /// Tempo de expiração do token em minutos
    /// </summary>
    public int ExpirationMinutes { get; }

    /// <summary>
    /// Tempo de expiração do refresh token em dias
    /// </summary>
    public int RefreshTokenExpirationDays { get; }

    /// <summary>
    /// Construtor que carrega as configurações do appsettings
    /// </summary>
    /// <param name="configurationWrapper">Wrapper de configuração</param>
    public JwtConfiguration(ConfigurationWrapper configurationWrapper)
    {
        SecretKey = configurationWrapper.GetValue<string>("Jwt:SecretKey") 
                   ?? throw new InvalidOperationException("Jwt:SecretKey não configurado");
        
        Issuer = configurationWrapper.GetValue<string>("Jwt:Issuer") 
                ?? throw new InvalidOperationException("Jwt:Issuer não configurado");
        
        Audience = configurationWrapper.GetValue<string>("Jwt:Audience") 
                  ?? throw new InvalidOperationException("Jwt:Audience não configurado");
        
        ExpirationMinutes = configurationWrapper.GetValue<int>("Jwt:ExpirationMinutes");
        if (ExpirationMinutes <= 0)
            ExpirationMinutes = 60; // Default: 1 hora

        RefreshTokenExpirationDays = configurationWrapper.GetValue<int>("Jwt:RefreshTokenExpirationDays");
        if (RefreshTokenExpirationDays <= 0)
            RefreshTokenExpirationDays = 7; // Default: 7 dias

        // Validar se a chave secreta tem tamanho mínimo
        if (SecretKey.Length < 32)
            throw new InvalidOperationException("Jwt:SecretKey deve ter pelo menos 32 caracteres");
    }
}