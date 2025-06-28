using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.EntityFrameworkCore;

namespace Kickoffa.API.Data.EntityFramework.Mapping
{
    public interface IUserEntityFrameworkMapping
    {
        public void Map(ModelBuilder modelBuilder);
    }

    public class UserEntityFrameworkMapping : IUserEntityFrameworkMapping
    {
        public void Map(ModelBuilder modelBuilder)
        {
            // Configurar propriedades adicionais da entidade User
            // As propriedades do IdentityUser já são configuradas automaticamente pelo Identity
            modelBuilder.Entity<User>(entity =>
            {
                // Configurar propriedades customizadas
                entity.Property(e => e.CreatedDateUtc)
                    .HasColumnName("CreatedDateUtc")
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastUpdatedDateUtc)
                    .HasColumnName("LastUpdatedDateUtc")
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configurar a tabela (opcional, o Identity já define como AspNetUsers)
                entity.ToTable("AspNetUsers");
            });
        }
    }
}
