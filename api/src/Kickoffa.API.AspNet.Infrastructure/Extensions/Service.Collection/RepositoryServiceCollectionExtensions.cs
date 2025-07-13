using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection
{
	public static class RepositoryServiceCollectionExtensions
	{
		/// <summary>
		/// Adiciona todos os repositórios ao container de DI
		/// </summary>
		/// <param name="services">Collection de serviços</param>
		/// <returns>IServiceCollection para chaining</returns>
		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			// Registrar mapeamentos do Entity Framework
			services.AddScoped<ICustomerEntityFrameworkMapping, CustomerEntityFrameworkMapping>();
			services.AddScoped<IUserEntityFrameworkMapping, UserEntityFrameworkMapping>();
			services.AddScoped<IChecklistEntityFrameworkMapping, ChecklistEntityFrameworkMapping>();
			services.AddScoped<ISectionEntityFrameworkMapping, SectionEntityFrameworkMapping>();
			services.AddScoped<IComponentEntityFrameworkMapping, ComponentEntityFrameworkMapping>();
			services.AddScoped<IComponentStatusEntityFrameworkMapping, ComponentStatusEntityFrameworkMapping>();
			services.AddScoped<IBriefingMediaEntityFrameworkMapping, BriefingMediaEntityFrameworkMapping>();
			services.AddScoped<IFileTypeEntityFrameworkMapping, FileTypeEntityFrameworkMapping>();
			services.AddScoped<IUploadComponentFileEntityFrameworkMapping, UploadComponentFileEntityFrameworkMapping>();
			services.AddScoped<ITextComponentEntityFrameworkMapping, TextComponentEntityFrameworkMapping>();
			services.AddScoped<IUploadComponentEntityFrameworkMapping, UploadComponentEntityFrameworkMapping>();
			services.AddScoped<IConfirmationComponentEntityFrameworkMapping, ConfirmationComponentEntityFrameworkMapping>();
			services.AddScoped<ICheckboxComponentEntityFrameworkMapping, CheckboxComponentEntityFrameworkMapping>();
			services.AddScoped<ISignatureComponentEntityFrameworkMapping, SignatureComponentEntityFrameworkMapping>();
			services.AddScoped<IUploadComponentFileTypeSizeEntityFrameworkMapping, UploadComponentFileTypeSizeEntityFrameworkMapping>();

			// Registrar repositório base genérico
			services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));

			// Registrar repositórios específicos
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<IFileTypeRepository, FileTypeRepository>();
			// UserRepository removido - agora usamos UserManager<User> do Identity

			// Repositórios do sistema de Checklist
			services.AddScoped<IChecklistRepository, ChecklistRepository>();
			services.AddScoped<ISectionRepository, SectionRepository>();
			services.AddScoped<IComponentRepository, ComponentRepository>();
			services.AddScoped<IComponentStatusRepository, ComponentStatusRepository>();
			services.AddScoped<IBriefingMediaRepository, BriefingMediaRepository>();
			services.AddScoped<IUploadComponentFileRepository, UploadComponentFileRepository>();
			services.AddScoped<ITextComponentRepository, TextComponentRepository>();
			services.AddScoped<IUploadComponentRepository, UploadComponentRepository>();
			services.AddScoped<IConfirmationComponentRepository, ConfirmationComponentRepository>();
			services.AddScoped<ICheckboxComponentRepository, CheckboxComponentRepository>();
			services.AddScoped<ISignatureComponentRepository, SignatureComponentRepository>();
			services.AddScoped<IUploadComponentFileTypeSizeRepository, UploadComponentFileTypeSizeRepository>();

			// Registrar Unit of Work
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}