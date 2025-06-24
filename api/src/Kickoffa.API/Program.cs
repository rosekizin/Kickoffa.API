using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.AspNet.Infrastructure.Extensions.ServiceCollection;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var configurationWrapper = new ConfigurationWrapper(builder.Configuration);

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: true));
	}); ;

// Add Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Database
builder.Services.AddDatabase(new PostgreDbConfiguration(configurationWrapper));

// Register services


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

app.UseAuthorization();

app.MapControllers();

app.Run();
