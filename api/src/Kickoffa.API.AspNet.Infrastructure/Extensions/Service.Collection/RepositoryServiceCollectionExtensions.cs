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

            // Registrar repositório base genérico
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Registrar repositórios específicos
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            // UserRepository removido - agora usamos UserManager<User> do Identity

            // TODO: Adicionar outros repositórios conforme forem criados
            // services.AddScoped<IChecklistRepository, ChecklistRepository>();
            // services.AddScoped<ISectionRepository, SectionRepository>();
            // services.AddScoped<IItemRepository, ItemRepository>();

            return services;
        }     
    }
}