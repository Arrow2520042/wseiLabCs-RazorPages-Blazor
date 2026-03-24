using ApplicationCore.Enums;

namespace ApplicationCore.Models.ContactAggregate;

public abstract class Contact : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address? Address { get; set; }
    public ContactStatus Status { get; set; } = ContactStatus.Active;
    public List<string> Tags { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
