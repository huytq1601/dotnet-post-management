using Microsoft.AspNetCore.Identity;

namespace EnityFrameworkRelationShip.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
