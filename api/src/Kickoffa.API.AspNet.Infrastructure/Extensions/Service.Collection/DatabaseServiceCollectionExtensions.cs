using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.EntityFramework.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection
{
	public static class DatabaseServiceCollectionExtensions
	{
		/// <summary>
		/// Adiciona a configuração do banco de dados PostgreSQL ao container de DI
		/// </summary>
		/// <param name="services">Collection de serviços</param>
		/// <param name="configuration">Configuração da aplicação</param>
		/// <returns>IServiceCollection para chaining</returns>
		public static IServiceCollection AddDatabase(this IServiceCollection services, IPostgreDbConfiguration postgreDbConfiguration)
		{
			var connectionString = postgreDbConfiguration.GetConnectionString();

			services.AddDbContext<KickoffaDbContext>(options =>
			{
				options.UseNpgsql(connectionString, npgsqlOptions =>
				{
					// Configurações específicas do PostgreSQL
					npgsqlOptions.EnableRetryOnFailure(
						maxRetryCount: 3,
						maxRetryDelay: TimeSpan.FromSeconds(30),
						errorCodesToAdd: null);

					npgsqlOptions.CommandTimeout(postgreDbConfiguration.CommandTimeout);
					npgsqlOptions.MigrationsAssembly("Kickoffa.API.Data");
				})
                .UseLazyLoadingProxies(useLazyLoadingProxies: true);

				// Configurações para ambiente de desenvolvimento
				var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				if (environment == "Development")
				{
					options.EnableSensitiveDataLogging();
					options.EnableDetailedErrors();
				}
			});

			services.AddScoped<IKickoffaDbContext>(provider => provider.GetRequiredService<KickoffaDbContext>());

			// Configurar pool de conexões para melhor performance. Fazer isso vai fazer com o Db context seja singleton
			//services.AddDbContextPool<KickoffaDbContext>(options =>
			//{
			//	options.UseNpgsql(connectionString);
			//});

			return services;
		}

		/*
        /// <summary>
        /// Adiciona apenas a configuração básica do DbContext sem pool
        /// </summary>
        /// <param name="services">Collection de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <returns>IServiceCollection para chaining</returns>
        public static IServiceCollection AddDatabaseBasic(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetConnectionString(configuration);

            services.AddDbContext<KickoffaDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("Kickoffa.API.Data");
                });
            });

            return services;
        }

        /// <summary>
        /// Executa as migrations pendentes no banco de dados
        /// </summary>
        /// <param name="serviceProvider">Provider de serviços</param>
        /// <returns>Task</returns>
        public static async Task RunDatabaseMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KickoffaDbContext>();

            try
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Console.WriteLine($"Aplicando {pendingMigrations.Count()} migrações pendentes...");
                    await context.Database.MigrateAsync();
                    Console.WriteLine("Migrações aplicadas com sucesso.");
                }
                else
                {
                    Console.WriteLine("Nenhuma migração pendente encontrada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao aplicar migrações: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Garante que o banco de dados seja criado (apenas para desenvolvimento)
        /// </summary>
        /// <param name="serviceProvider">Provider de serviços</param>
        /// <returns>Task</returns>
        public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KickoffaDbContext>();

            try
            {
                await context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar banco de dados: {ex.Message}");
                throw;
            }
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            // Tentar obter da configuração primeiro
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            // Fallback para variáveis de ambiente
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "kickoffa";
            var username = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";
            var sslMode = Environment.GetEnvironmentVariable("DB_SSL_MODE") ?? "Prefer";

            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true;";
        }
        */

		private static void AddEntityFrameworkMappings(IServiceCollection services)
		{
			services.AddSingleton<IUserEntityFrameworkMapping, UserEntityFrameworkMapping>();
			services.AddSingleton<ICustomerEntityFrameworkMapping, CustomerEntityFrameworkMapping>();
			services.AddSingleton<IChecklistEntityFrameworkMapping, ChecklistEntityFrameworkMapping>();
			services.AddSingleton<ISectionEntityFrameworkMapping, SectionEntityFrameworkMapping>();
			services.AddSingleton<IComponentEntityFrameworkMapping, ComponentEntityFrameworkMapping>();
			services.AddSingleton<IComponentStatusEntityFrameworkMapping, ComponentStatusEntityFrameworkMapping>();
			services.AddSingleton<IBriefingMediaEntityFrameworkMapping, BriefingMediaEntityFrameworkMapping>();
			services.AddSingleton<IFileTypeEntityFrameworkMapping, FileTypeEntityFrameworkMapping>();
		}
	}
}