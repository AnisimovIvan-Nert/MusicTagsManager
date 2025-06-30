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

    public Stream OpenReadStream(IResourceIdentifier identifier)
    {
        var repository = repositories
            .First(repository => repository.Contains(identifier));

        return repository.GetStreamAccess(identifier).OpenRead();
    }

    public Stream OpenWriteStream(IResourceIdentifier identifier)
    {
        var repository = repositories
            .First(repository => repository.Contains(identifier));

        return repository.GetStreamAccess(identifier).OpenWrite();
    }
}