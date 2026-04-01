using ApplicationCore.Enums;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models.ContactAggregate;
using Infrastructure.Memory.Repositories;

namespace UnitTest;

public class MemoryGenericRepositoryAsyncTest
{
    private readonly IGenericRepositoryAsync<Person> _repo;
    private readonly Person _person1;
    private readonly Person _person2;

    public MemoryGenericRepositoryAsyncTest()
    {
        _repo = new MemoryGenericRepositoryAsync<Person>();
        _person1 = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Gender = Gender.Male
        };
        _person2 = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Gender = Gender.Female
        };
    }

    [Fact]
    public async Task AddPersonTestAsync()
    {
        var added = await _repo.AddAsync(_person1);

        var found = await _repo.FindByIdAsync(added.Id);

        Assert.NotNull(found);
        Assert.Equal(added.Id, found.Id);
        Assert.Equal("John", found.FirstName);
        Assert.Equal("Doe", found.LastName);
    }

    [Fact]
    public async Task AddAssignsGuidWhenEmptyAsync()
    {
        var person = new Person { FirstName = "Random" };
        Assert.Equal(Guid.Empty, person.Id);

        var added = await _repo.AddAsync(person);

        Assert.NotEqual(Guid.Empty, added.Id);
    }

    [Fact]
    public async Task FindAllReturnsAllAddedAsync()
    {
        await _repo.AddAsync(_person1);
        await _repo.AddAsync(_person2);

        var all = (await _repo.FindAllAsync()).ToList();

        Assert.Equal(2, all.Count);
        Assert.Contains(all, p => p.FirstName == "John");
        Assert.Contains(all, p => p.FirstName == "Jane");
    }

    [Fact]
    public async Task FindByIdReturnsNullWhenNotFoundAsync()
    {
        var result = await _repo.FindByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveByIdDeletesEntityAsync()
    {
        var added = await _repo.AddAsync(_person1);
        await _repo.AddAsync(_person2);

        await _repo.RemoveByIdAsync(added.Id);

        var all = (await _repo.FindAllAsync()).ToList();
        Assert.Single(all);
        Assert.DoesNotContain(all, p => p.Id == added.Id);
    }

    [Fact]
    public async Task RemoveByIdThrowsWhenNotFoundAsync()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _repo.RemoveByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateReplacesEntityAsync()
    {
        var added = await _repo.AddAsync(_person1);
        added.FirstName = "Modified";

        var updated = await _repo.UpdateAsync(added);
        var found = await _repo.FindByIdAsync(added.Id);

        Assert.Equal("Modified", updated.FirstName);
        Assert.Equal("Modified", found?.FirstName);
    }

    [Fact]
    public async Task UpdateThrowsWhenNotFoundAsync()
    {
        var person = new Person { Id = Guid.NewGuid(), FirstName = "Ghost" };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _repo.UpdateAsync(person));
    }

    [Fact]
    public async Task FindPagedReturnsCorrectPageAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            await _repo.AddAsync(new Person { FirstName = $"Person{i}" });
        }

        var page1 = await _repo.FindPagedAsync(1, 2);
        var page2 = await _repo.FindPagedAsync(2, 2);
        var page3 = await _repo.FindPagedAsync(3, 2);

        Assert.Equal(5, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(2, page2.Items.Count);
        Assert.Single(page3.Items);
        Assert.True(page1.HasNext);
        Assert.False(page1.HasPrevious);
        Assert.True(page3.HasPrevious);
        Assert.False(page3.HasNext);
    }
}
