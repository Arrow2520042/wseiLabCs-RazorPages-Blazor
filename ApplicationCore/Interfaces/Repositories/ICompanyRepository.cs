using ApplicationCore.Models.ContactAggregate;

namespace ApplicationCore.Interfaces.Repositories;

public interface ICompanyRepository : IGenericRepositoryAsync<Company>
{
    Task<IEnumerable<Company>> FindByNameAsync(string name);
    Task<Company?> FindByNipAsync(string nip);
    Task<IEnumerable<Person>> FindEmployeesAsync(Guid companyId);
}
