namespace ApplicationCore.Models.ContactAggregate;

public class Note : EntityBase
{
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
