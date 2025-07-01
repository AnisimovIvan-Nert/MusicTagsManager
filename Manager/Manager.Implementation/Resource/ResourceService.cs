using Manager.Resource;

namespace Manager.Implementation.Resource;

public class ResourceService(
    params IResourceRepository[] repositories)
    : IResourceService
{
    public IEnumerable<IResource> GetAll()
    {
        return repositories.SelectMany(repository => repository.GetAll());
    }

    public IResource Get(IResourceIdentifier identifier)
    {
        var repository = FindParentRepository(identifier);
        return repository.Get(identifier);
    }

    public Stream OpenReadStream(IResourceIdentifier identifier)
    {
        var repository = FindParentRepository(identifier);
        return repository.GetStreamAccess(identifier).OpenRead();
    }

    public Stream OpenWriteStream(IResourceIdentifier identifier)
    {
        var repository = FindParentRepository(identifier);
        return repository.GetStreamAccess(identifier).OpenWrite();
    }

    private IResourceRepository FindParentRepository(IResourceIdentifier identifier)
    {
        return repositories.First(repository => repository.Contains(identifier));
    }
}