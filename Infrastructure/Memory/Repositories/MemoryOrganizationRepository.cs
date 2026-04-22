using ApplicationCore.Enums;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;

namespace Infrastructure.Memory.Repositories;

public class MemoryOrganizationRepository : MemoryGenericRepositoryAsync<Organization>, IOrganizationRepository
{
    public Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        var result = _data.Values.Where(o => o.Type == type);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> FindMembersAsync(Guid organizationId)
    {
        var organization = _data.Values.FirstOrDefault(o => o.Id == organizationId);
        var members = organization?.Members ?? new List<Person>();
        return Task.FromResult<IEnumerable<Person>>(members);
    }
}
