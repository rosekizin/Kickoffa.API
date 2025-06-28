using Kickoffa.API.AspNet.Infrastructure.Configuration.Authentication;
using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.AspNet.Infrastructure.Extensions.ServiceCollection;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Kickoffa.API.Middlewares;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var configurationWrapper = new ConfigurationWrapper(builder.Configuration);

// Configurar JWT
var jwtConfiguration = new JwtConfiguration(configurationWrapper);

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

// Add Authentication & Authorization
builder.Services.AddJwtAuthentication(jwtConfiguration);

// Register Database
builder.Services.AddDatabase(new PostgreDbConfiguration(configurationWrapper));
builder.Services.AddRepositories();

// Configure Identity
builder.Services.AddIdentityConfiguration();

// Register services
builder.Services.AddApplicationServices();

// Register JWT service
builder.Services.AddScoped<IJwtTokenService>(provider =>
    new JwtTokenService(
        jwtConfiguration.SecretKey,
        jwtConfiguration.Issuer,
        jwtConfiguration.Audience,
        jwtConfiguration.ExpirationMinutes
    ));

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
