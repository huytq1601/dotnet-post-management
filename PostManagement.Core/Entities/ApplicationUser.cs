using Microsoft.AspNetCore.Identity;

namespace PostManagement.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
