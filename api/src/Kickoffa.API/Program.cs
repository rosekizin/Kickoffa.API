using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.AspNet.Infrastructure.Extensions.ServiceCollection;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var configurationWrapper = new ConfigurationWrapper(builder.Configuration);

// JWT removido - ASP.NET Core Identity gerencia autenticação

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: true));
	}); ;

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

// Authentication & Authorization gerenciados pelo Identity

// Register Database
builder.Services.AddDatabase(new PostgreDbConfiguration(configurationWrapper));
builder.Services.AddRepositories();

// Configure Identity
builder.Services.AddIdentityConfiguration();

// Register services
builder.Services.AddApplicationServices();

// JWT service removido - ASP.NET Core Identity gerencia autenticação automaticamente

// Handlers


// Factories


var app = builder.Build();

if (app.Environment.IsDevelopment())
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

app.Run();
