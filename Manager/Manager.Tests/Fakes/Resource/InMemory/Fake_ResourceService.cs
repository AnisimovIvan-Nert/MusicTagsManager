using Manager.Resource;

namespace Manager.Tests.Fakes.Resource.InMemory;

public class Fake_ResourceService(
    Fake_ResourceRepository repository)
    : IResourceService
{
    public IEnumerable<IResource> GetAll()
    {
        return repository.GetAll();
    }

    public IResource Get(IResourceIdentifier identifier)
    {
        return repository.Get(identifier);
    }

    public Stream OpenReadStream(IResourceIdentifier identifier)
    {
        return repository.GetStreamAccess(identifier).OpenRead();
    }

    public Stream OpenWriteStream(IResourceIdentifier identifier)
    {
        return repository.GetStreamAccess(identifier).OpenWrite();
    }
}