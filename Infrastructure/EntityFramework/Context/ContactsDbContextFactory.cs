using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContextFactory : IDesignTimeDbContextFactory<ContactsDbContext>
{
    public ContactsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContactsDbContext>();
        optionsBuilder.UseSqlite("Data Source=contacts.db");
        return new ContactsDbContext(optionsBuilder.Options);
    }
}
