using Microsoft.Extensions.DependencyInjection;
using Kickoffa.API.Data.Repositories.Base;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Data.Repositories;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.ServiceCollection
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
            // Registrar repositório base genérico
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Registrar repositórios específicos
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // TODO: Adicionar outros repositórios conforme forem criados
            // services.AddScoped<IChecklistRepository, ChecklistRepository>();
            // services.AddScoped<ISectionRepository, SectionRepository>();
            // services.AddScoped<IItemRepository, ItemRepository>();

            return services;
        }     
    }
}