using Amazon.S3;
using Kickoffa.API.Application.Configuration;
using Kickoffa.API.Application.Factories;
using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Application.Interfaces.Factories;
using Kickoffa.API.Application.Services;
using Kickoffa.API.Application.Services.AppUser;
using Kickoffa.API.Application.Services.Checklists;
using Kickoffa.API.Application.Services.Email;
using Kickoffa.API.Application.Wrappers;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Models.AppUser;
using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection;

/// <summary>
/// Extensões para configuração dos serviços da Application
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
	/// <summary>
	/// Adiciona todos os serviços da camada Application
	/// </summary>
	/// <param name="services">Collection de serviços</param>
	/// <returns>IServiceCollection para chaining</returns>
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		// Registrar factories
		services.AddScoped<ISectionFactory, SectionFactory>();
		services.AddScoped<IFileTypeFactory, FileTypeFactory>();
		services.AddScoped<IComponentFactory, ComponentFactory>();
		services.AddScoped<IChecklistFactory, ChecklistFactory>();
		services.AddScoped<IBriefingMediaFactory, BriefingMediaFactory>();
		services.AddScoped<ICreateCustomerFactory, CreateCustomerFactory>();
		services.AddScoped<IUploadComponentFileTypeSizeFactory, UploadComponentFileTypeSizeFactory>();

		// Registrar services
		services.AddScoped<ICustomerService, CustomerService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();

		services.AddScoped<IFileTypeService, FileTypeService>();
		services.AddScoped<IFileUploadService, FileUploadService>();
		services.AddScoped<IImageProxyService, ImageProxyService>();
		services.AddScoped<ITipTapContentParserService, TipTapContentParserService>();
		services.AddScoped<IChecklistService, ChecklistService>();
		services.AddScoped<ICreateChecklistService, CreateChecklistService>();
		services.AddScoped<IUpdateChecklistService, UpdateChecklistService>();
		services.AddScoped<IMapChecklistToResponse, MapChecklistToResponse>();
        services.AddScoped<IAddBriefingMediaService, AddBriefingMediaService>();
        services.AddScoped<IUpdateBriefingSectionService, UpdateBriefingSectionService>();
		services.AddScoped<IUpdateChecklistSectionService, UpdateChecklistSectionService>();

		// Register wrappers
		services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
		services.AddScoped<ISignInManagerWrapper, SignInManagerWrapper>();

		// HttpContextAccessor necessário para CurrentUserService
		services.AddHttpContextAccessor();

		// Configurar AWS S3
		services.AddAwsS3Services();

		return services;
	}

	/// <summary>
	/// Configura ASP.NET Core Identity
	/// </summary>
	/// <param name="services">Collection de serviços</param>
	/// <returns>IServiceCollection para chaining</returns>
	public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
	{
		// Configurar Identity com User e Role personalizados usando long como chave
		services.AddIdentity<User, Role>(options =>
		{
			// Configurações de senha
			options.Password.RequireDigit = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireUppercase = false;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequiredLength = 6;
			options.Password.RequiredUniqueChars = 1;

			// Configurações de usuário
			options.User.RequireUniqueEmail = true;
			options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

			// Configurações de lockout
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
			options.Lockout.MaxFailedAccessAttempts = 5;
			options.Lockout.AllowedForNewUsers = true;

			// Configurações de sign-in
			options.SignIn.RequireConfirmedEmail = false;
			options.SignIn.RequireConfirmedPhoneNumber = false;
		})
		.AddEntityFrameworkStores<KickoffaDbContext>()
		.AddDefaultTokenProviders();

		// Configurar autenticação com cookies
		services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = "/api/auth/login";
			options.LogoutPath = "/api/auth/logout";
			options.AccessDeniedPath = "/api/auth/access-denied";
			options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
			options.SlidingExpiration = true;
			options.Cookie.HttpOnly = true;
			options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
			options.Cookie.SameSite = SameSiteMode.Strict;
			options.Cookie.Name = "KickoffaAuth";
		});

		return services;
	}

	/// <summary>
	/// Configura os serviços do AWS S3
	/// </summary>
	/// <param name="services">Collection de serviços</param>
	/// <returns>IServiceCollection para chaining</returns>
	public static IServiceCollection AddAwsS3Services(this IServiceCollection services)
	{
		// Configurar AWS S3 Configuration
		services.AddOptions<AwsS3Configuration>()
			.BindConfiguration("AWS:S3")
			.ValidateDataAnnotations()
			.ValidateOnStart();

		// Registrar cliente S3
		services.AddScoped<IAmazonS3>(provider =>
		{
			var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AwsS3Configuration>>().Value;

			var s3Config = new Amazon.S3.AmazonS3Config
			{
				RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(config.Region)
			};

			// Se estiver usando LocalStack para desenvolvimento
			if (config.UseLocalStack && !string.IsNullOrWhiteSpace(config.LocalStackUrl))
			{
				s3Config.ServiceURL = config.LocalStackUrl;
				s3Config.ForcePathStyle = true;
			}

			return new Amazon.S3.AmazonS3Client(config.AccessKey, config.SecretKey, s3Config);
		});

		return services;
	}
}