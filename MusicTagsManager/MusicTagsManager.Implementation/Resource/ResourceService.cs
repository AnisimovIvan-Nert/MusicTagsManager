using MusicTagsManager.Resource;

namespace MusicTagsManager.Implementation.Resource;

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

    public string GetResourceExtension(IResourceIdentifier identifier)
    {
        FindParentRepository(identifier);
        return Path.GetExtension(identifier.Identifier);
    }

    private IResourceRepository FindParentRepository(IResourceIdentifier identifier)
    {
        return repositories.First(repository => repository.Contains(identifier));
    }
}