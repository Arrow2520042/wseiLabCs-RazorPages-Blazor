using ApplicationCore.Dto;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;

namespace Infrastructure.Memory.Repositories;

public class MemoryGenericRepositoryAsync<T> : IGenericRepositoryAsync<T>
    where T : EntityBase
{
    protected readonly Dictionary<Guid, T> _data = new();

    public Task<T?> FindByIdAsync(Guid id)
    {
        _data.TryGetValue(id, out var value);
        return Task.FromResult(value);
    }

    public Task<IEnumerable<T>> FindAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_data.Values.ToList());
    }

    public Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var allItems = _data.Values.ToList();
        var items = allItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<T>(items, allItems.Count, page, pageSize);
        return Task.FromResult(result);
    }

    public Task<T> AddAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        if (!_data.ContainsKey(entity.Id))
        {
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");
        }
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task RemoveByIdAsync(Guid id)
    {
        if (!_data.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Entity with id {id} not found.");
        }
        _data.Remove(id);
        return Task.CompletedTask;
    }
}
