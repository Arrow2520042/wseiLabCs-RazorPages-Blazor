using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;

namespace Infrastructure.Memory.Repositories;

public class MemoryCompanyRepository : MemoryGenericRepositoryAsync<Company>, ICompanyRepository
{
    public Task<IEnumerable<Company>> FindByNameAsync(string name)
    {
        var result = _data.Values.Where(c =>
            c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result);
    }

    public Task<Company?> FindByNipAsync(string nip)
    {
        var result = _data.Values.FirstOrDefault(c => c.Nip == nip);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> FindEmployeesAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }
}
