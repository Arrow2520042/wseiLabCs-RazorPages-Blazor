using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repositories;

namespace Infrastructure.Memory;

public class MemoryContactUnitOfWork : IContactUnitOfWork
{
    private readonly IPersonRepository _persons;
    private readonly ICompanyRepository _companies;
    private readonly IOrganizationRepository _organizations;

    public MemoryContactUnitOfWork(
        IPersonRepository persons,
        ICompanyRepository companies,
        IOrganizationRepository organizations)
    {
        _persons = persons;
        _companies = companies;
        _organizations = organizations;
    }

    public IPersonRepository Persons => _persons;
    public ICompanyRepository Companies => _companies;
    public IOrganizationRepository Organizations => _organizations;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task<int> SaveChangesAsync() => Task.FromResult(0);

    public Task BeginTransactionAsync() => Task.CompletedTask;

    public Task CommitTransactionAsync() => Task.CompletedTask;

    public Task RollbackTransactionAsync() => Task.CompletedTask;
}
