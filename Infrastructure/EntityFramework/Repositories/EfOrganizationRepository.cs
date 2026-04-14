using ApplicationCore.Enums;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfOrganizationRepository(ContactsDbContext context)
    : EfGenericRepositoryAsync<Organization>(context.Organizations), IOrganizationRepository
{
    public async Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        return await context.Organizations
            .AsNoTracking()
            .Where(o => o.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> FindMembersAsync(Guid organizationId)
    {
        return await context.People
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();
    }
}
