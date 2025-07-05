using Microsoft.Extensions.DependencyInjection;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Data.EntityFramework.Mapping;

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
            services.AddScoped<IItemEntityFrameworkMapping, ItemEntityFrameworkMapping>();
            services.AddScoped<IItemStatusEntityFrameworkMapping, ItemStatusEntityFrameworkMapping>();
            services.AddScoped<IBriefingMediaEntityFrameworkMapping, BriefingMediaEntityFrameworkMapping>();
            services.AddScoped<IFileTypeEntityFrameworkMapping, FileTypeEntityFrameworkMapping>();
            services.AddScoped<IUploadItemFileTypeEntityFrameworkMapping, UploadItemFileTypeEntityFrameworkMapping>();
            services.AddScoped<IUploadItemFileEntityFrameworkMapping, UploadItemFileEntityFrameworkMapping>();

            // Registrar repositório base genérico
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Registrar repositórios específicos
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IFileTypeRepository, FileTypeRepository>();
            // UserRepository removido - agora usamos UserManager<User> do Identity

            // Repositórios do sistema de Checklist
            services.AddScoped<IChecklistRepository, ChecklistRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemStatusRepository, ItemStatusRepository>();
            services.AddScoped<IBriefingMediaRepository, BriefingMediaRepository>();
            services.AddScoped<IUploadItemFileRepository, UploadItemFileRepository>();

            return services;
        }     
    }
}