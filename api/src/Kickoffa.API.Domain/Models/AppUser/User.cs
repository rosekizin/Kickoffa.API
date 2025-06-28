using System.Text.RegularExpressions;
using Kickoffa.API.Domain.Models.Base;

namespace Kickoffa.API.Domain.Models.AppUser
{
    public class User : BaseEntity<User>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Role { get; private set; }

        public User(string name, string email, string password, string role = "freelancer")
        {
            ValidateEmail(email);
            
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email;
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Role = role ?? "freelancer";
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório", nameof(email));

            // Regex para validação de email
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);
            
            if (!emailRegex.IsMatch(email))
                throw new ArgumentException("Email deve ter um formato válido", nameof(email));
        }

        public void UpdatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password é obrigatório", nameof(newPassword));
                
            Password = newPassword;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Nome é obrigatório", nameof(newName));
                
            Name = newName;
        }

        public void UpdateEmail(string newEmail)
        {
            ValidateEmail(newEmail);
            Email = newEmail;
        }
    }
}
