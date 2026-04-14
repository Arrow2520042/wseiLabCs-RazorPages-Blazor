namespace ApplicationCore.Models.ContactAggregate;

public class Company : Contact
{
    public string Name { get; set; } = string.Empty;
    public string Nip { get; set; } = string.Empty;
    public string? Website { get; set; }
    public List<Person> Employees { get; set; } = new();
}
