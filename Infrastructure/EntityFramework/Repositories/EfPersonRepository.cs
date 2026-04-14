using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository(ContactsDbContext context)
    : EfGenericRepositoryAsync<Person>(context.People), IPersonRepository
{
    public async Task<IEnumerable<Person>> FindByEmployerAsync(Guid companyId)
    {
        return await context.People
            .AsNoTracking()
            .Where(p => p.EmployerId == companyId)
            .Include(p => p.Employer)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        return await context.People
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .Include(p => p.Organization)
            .ToListAsync();
    }
}
