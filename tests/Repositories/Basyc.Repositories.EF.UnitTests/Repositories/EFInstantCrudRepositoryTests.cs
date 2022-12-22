using FluentAssertions;
//using Kontrer.Shared.Repositories.EF.Tests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Basyc.Repositories.EF.Tests.Repositories;

public class EFInstantCrudRepositoryTests
{
    //private readonly PersonEFCrudRepository peopleRepo;
    //private readonly CarEFCrudRepository carRepo;
    private readonly TestUnitOfWork unitOfWork;

    private readonly DbContextOptions<TestDbContext> dbOptions;
    private readonly TestDbContext dbContext;

    public EFInstantCrudRepositoryTests()
    {
        dbOptions = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase("TestDbContextInMemoryDb").Options;
        dbContext = new TestDbContext(dbOptions);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        unitOfWork = new TestUnitOfWork(dbContext);
    }

    [Fact]
    public async Task Proxy_Id_Set_Should_Work()
    {
        var personToRepo = new PersonModel();
        personToRepo.Id = new Random().Next();
        personToRepo.Age = new DateTime(2000, 1, 1);
        personToRepo.Name = "John";
        await unitOfWork.People.InstaAddAsync(personToRepo);

        //Need to create a new repo since hidden SetEntityId is not used when entity is cached.
        var newRepo = new TestUnitOfWork(new TestDbContext(dbOptions)).People;

        await newRepo.InstaRemoveAsync(personToRepo.Id);
        (await newRepo.TryGetAsync(personToRepo.Id)).Should().Be(default);
    }

    [Fact]
    public async Task Proxy_Id_Get_Should_Work()
    {
        var personToRepo = new PersonModel();
        personToRepo.Id = new Random().Next();
        personToRepo.Age = new DateTime(2000, 1, 1);
        personToRepo.Name = "John";
        await unitOfWork.People.InstaAddAsync(personToRepo);
        var personFromRepo = await unitOfWork.People.GetAsync(personToRepo.Id);
        personFromRepo.Id.Should().Be(personToRepo.Id);
    }

    [Fact]
    public async Task Should_Update_After_Add_Without_Save()
    {
        string newName = "2";

        var model = new PersonModel();
        model.Id = new Random().Next();
        model.Name = "1";

        await unitOfWork.People.InstaAddAsync(model);
        model.Name = newName;
        await unitOfWork.People.InstaUpdateAsync(model);

        var newModel = await unitOfWork.People.TryGetAsync(model.Id);
        newModel.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Should_Update_After_Add_With_Save()
    {
        string newName = "2";

        var personToRepo = new PersonModel();
        personToRepo.Id = new Random().Next();
        personToRepo.Name = "1";

        await unitOfWork.People.InstaAddAsync(personToRepo);
        personToRepo.Name = newName;
        await unitOfWork.People.InstaUpdateAsync(personToRepo);

        var personFromRepo = await unitOfWork.People.TryGetAsync(personToRepo.Id);
        personFromRepo.Name.Should().Be(newName);
    }
}
