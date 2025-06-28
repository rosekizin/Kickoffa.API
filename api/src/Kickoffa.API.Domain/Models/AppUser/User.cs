using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Domain.Models.AppUser
{
    public class User : IdentityUser<long>
    {
        // Propriedades adicionais além das do IdentityUser
        public DateTime CreatedDateUtc { get; set; }
        public DateTime LastUpdatedDateUtc { get; set; }

        public User()
        {
            CreatedDateUtc = DateTime.UtcNow;
            LastUpdatedDateUtc = DateTime.UtcNow;
        }

        public User(string email) : this()
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            UserName = email; // IdentityUser usa UserName como identificador único
            NormalizedEmail = email.ToUpperInvariant();
            NormalizedUserName = email.ToUpperInvariant();
        }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email é obrigatório", nameof(newEmail));

            Email = newEmail;
            UserName = newEmail; // Manter UserName sincronizado
            NormalizedEmail = newEmail.ToUpperInvariant();
            NormalizedUserName = newEmail.ToUpperInvariant();
            LastUpdatedDateUtc = DateTime.UtcNow;
        }
    }
}
