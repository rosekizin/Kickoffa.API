using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.AspNet.Infrastructure.ExceptionHandling;
using Kickoffa.API.AspNet.Infrastructure.Extensions;
using Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Kickoffa.API.Contracts.Newtonsoft;
using Kickoffa.API.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace Kickoffa.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Wrappers
            var configurationWrapper = new ConfigurationWrapper(builder.Configuration);
            var hostEnvironmentWrapper = new HostEnvironmentWrapper(builder.Environment);

            // Configurar Serilog como provedor de logging principal
            // IMPORTANTE: Deve ser configurado antes de outras configurações para capturar logs de inicialização
            builder.AddSerilogLogging(configurationWrapper, hostEnvironmentWrapper);

            // Para configuração simplificada apenas console, use:
            // builder.AddSerilogConsoleLogging();

            // JWT removido - ASP.NET Core Identity gerencia autenticação

            builder.Services
                .AddControllers(options =>
                {
                    options.Filters.Add<ExceptionFilter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.Converters.Add(new CustomerRequestConverter());
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.None; // ou Auto, se quiser polimorfismo com $type
                });

            // Add Swagger (Swashbuckle)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS - Configurado para HttpOnly cookies
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Frontend URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Essencial para HttpOnly cookies
                });
            });

            // Register Database
            builder.Services.AddDatabase(new PostgreDbConfiguration(configurationWrapper));
            builder.Services.AddRepositories();

            // Configure Identity
            // Authentication & Authorization gerenciados pelo Identity
            // JWT service removido - ASP.NET Core Identity gerencia autenticação automaticamente
            builder.Services.AddIdentityConfiguration();

            // Register services
            builder.Services.AddApplicationServices();
            builder.Services.AddSingleton<IResultToActionResultConverter, ResultToActionResultConverter>();

            // Handlers
            builder.Services.AddErrorHandlers();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // Factories


            var app = builder.Build();

            // precisa vir antes de qualquer middleware que possa gerar exceções, como:
            // UseAuthentication(), UseAuthorization(), MapControllers()
            app.UseExceptionHandler(options => { });

            if (hostEnvironmentWrapper.IsDevelopment())
            {
                // Enable Swagger UI in dev
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("swagger/v1/swagger.json", "Kickoffa API V1");
                    options.RoutePrefix = string.Empty; // Swagger UI na raiz:
                });
            }

            app.UseHttpsRedirection();

            // Configurar CORS
            app.UseCors("AllowFrontend");

            // Configurar pipeline de autenticação
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Log de inicialização da aplicação
            Log.Information("🚀 Kickoffa API iniciada com sucesso!");
            Log.Information("Ambiente: {Environment}", app.Environment.EnvironmentName);
            Log.Information("URLs: {Urls}", string.Join(", ", app.Urls));

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "💥 Aplicação falhou ao inicializar");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}