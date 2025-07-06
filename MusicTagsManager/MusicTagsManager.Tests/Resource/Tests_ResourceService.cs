using MusicTagsManager.Implementation.Resource;
using MusicTagsManager.Resource;
using MusicTagsManager.Tests.Fakes.Resource.InMemory;

namespace MusicTagsManager.Tests.Resource;

public class Tests_ResourceService
{
    private const int FakeRepositoryCount = 10;

    private readonly Fake_ResourceRepository[] _fakeRepositories;
    private readonly IResourceRepository[] _repositories;

    public Tests_ResourceService()
    {
        var resourceFactory = new ResourceFactory();
        _fakeRepositories = new Fake_ResourceRepository[FakeRepositoryCount];

        for (var i = 0; i < FakeRepositoryCount; i++)
        {
            var identifier = i.ToString();
            var resource = resourceFactory.Create(identifier, identifier, identifier);
            var repository = new Fake_ResourceRepository(resource);
            _fakeRepositories[i] = repository;
        }

        _repositories = _fakeRepositories.Cast<IResourceRepository>().ToArray();
    }

    [Fact]
    public void GetAll_ReturnAllResourcesFromRepositories()
    {
        var service = new ResourceService(_repositories);
        var allResources = _fakeRepositories
            .SelectMany(repository => repository.GetAll()).ToArray();

        var resourcesFromService = service.GetAll();

        Assert.Equal(allResources.Length, resourcesFromService.Count());
    }

    [Fact]
    public void OperReadStream_ValidResource_ReturnReadStreamForResource()
    {
        var repository = _fakeRepositories.First();
        var service = new ResourceService(repository);
        var resource = repository.GetAll().First();
        using var actualStream = repository.GetStreamAccess(resource).OpenRead();

        using var returnedStream = service.OpenReadStream(resource);

        Assert.Equal(actualStream, returnedStream);
    }

    [Fact]
    public void OperReadStream_InvalidResource_ThrowInvalidOperationException()
    {
        var validRepository = _fakeRepositories.First();
        var invalidRepository = _fakeRepositories.Skip(1).First();
        var invalidResource = invalidRepository.GetAll().First();
        var service = new ResourceService(validRepository);

        Assert.Throws<InvalidOperationException>(() => service.OpenReadStream(invalidResource));
    }

    [Fact]
    public void OpenWriteStream_ValidResource_ReturnWriteStreamForResource()
    {
        var repository = _fakeRepositories.First();
        var service = new ResourceService(repository);
        var resource = repository.GetAll().First();
        using var actualStream = repository.GetStreamAccess(resource).OpenWrite();

        using var returnedStream = service.OpenWriteStream(resource);

        Assert.Equal(actualStream, returnedStream);
    }

    [Fact]
    public void OperWriteStream_InvalidResource_ThrowInvalidOperationException()
    {
        var validRepository = _fakeRepositories.First();
        var invalidRepository = _fakeRepositories.Skip(1).First();
        var invalidResource = invalidRepository.GetAll().First();
        var service = new ResourceService(validRepository);

        Assert.Throws<InvalidOperationException>(() => service.OpenWriteStream(invalidResource));
    }
}