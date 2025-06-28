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
            // Configuração da tabela
            modelBuilder.Entity<User>().ToTable("Users");

            // Configuração da chave primária
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id)
                .HasColumnName("Id")
                .HasColumnType("uuid")
                .IsRequired()
                .ValueGeneratedNever(); // Guid é gerado pela aplicação

            modelBuilder.Entity<User>().Property(u => u.Name)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<User>().Property(u => u.Email)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<User>().Property(u => u.Password)
                .HasColumnName("Password")
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<User>().Property(u => u.Role)
                .HasColumnName("Role")
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue("freelancer");

            // Configuração das propriedades da BaseEntity
            modelBuilder.Entity<User>().Property(u => u.CreatedDateUtc)
                .HasColumnName("CreatedDateUtc")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<User>().Property(u => u.LastUpdatedDateUtc)
                .HasColumnName("LastUpdatedDateUtc")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes
            modelBuilder.Entity<User>().HasIndex(e => e.Email).IsUnique();
        }
    }
}
