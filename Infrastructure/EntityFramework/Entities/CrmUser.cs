using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.EntityFramework.Entities;

public class CrmUser : IdentityUser, ISystemUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public SystemUserStatus Status { get; set; } = SystemUserStatus.PendingActivation;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    string ISystemUser.Id => Id;
    string ISystemUser.Email => Email ?? string.Empty;

    public void Activate()
    {
        if (Status == SystemUserStatus.Inactive)
        {
            Status = SystemUserStatus.Active;
            DeactivatedAt = null;
        }
    }

    public void Deactivate(DateTime now)
    {
        if (Status == SystemUserStatus.Active)
        {
            Status = SystemUserStatus.Inactive;
            DeactivatedAt = now;
        }
    }

    public void RegisterLogin(DateTime now)
    {
        LastLoginAt = now;
    }
}
