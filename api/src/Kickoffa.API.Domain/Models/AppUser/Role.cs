using Microsoft.AspNetCore.Identity;

namespace Kickoffa.API.Domain.Models.AppUser
{
    public class Role : IdentityRole<long>
    {
        public DateTime CreatedDateUtc { get; set; }
        public DateTime LastUpdatedDateUtc { get; set; }

        public Role() : base()
        {
            CreatedDateUtc = DateTime.UtcNow;
            LastUpdatedDateUtc = DateTime.UtcNow;
        }

        public Role(string roleName) : base(roleName)
        {
            CreatedDateUtc = DateTime.UtcNow;
            LastUpdatedDateUtc = DateTime.UtcNow;
        }
    }
}