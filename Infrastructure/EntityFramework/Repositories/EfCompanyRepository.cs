using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfCompanyRepository(ContactsDbContext context)
    : EfGenericRepositoryAsync<Company>(context.Companies), ICompanyRepository
{
    public async Task<IEnumerable<Company>> FindByNameAsync(string name)
    {
        return await context.Companies
            .AsNoTracking()
            .Where(c => c.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<Company?> FindByNipAsync(string nip)
    {
        return await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nip == nip);
    }

    public async Task<IEnumerable<Person>> FindEmployeesAsync(Guid companyId)
    {
        return await context.People
            .AsNoTracking()
            .Where(p => p.EmployerId == companyId)
            .ToListAsync();
    }
}
