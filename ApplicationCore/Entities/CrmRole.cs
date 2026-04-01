using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    public class CrmRole : IdentityRole
    {
        public string Description { get; set; } = string.Empty;
    }
}
