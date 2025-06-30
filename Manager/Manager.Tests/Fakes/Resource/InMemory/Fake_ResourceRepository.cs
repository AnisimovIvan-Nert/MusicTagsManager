using Manager.Resource;

namespace Manager.Tests.Fakes.Resource.InMemory;

public class Fake_ResourceRepository(
    IResource resource) 
    : IResourceRepository
{
    private IResourceStreamAccess? _streamAccess;

    public IEnumerable<IResource> GetAll()
    {
        yield return resource;
    }

    public IResourceStreamAccess GetStreamAccess(IResourceIdentifier identifier)
    {
        _streamAccess ??= new Fake_ResourceStreamAccess();
        return _streamAccess;
    }

    public bool Contains(IResourceIdentifier identifier)
    {
        return identifier == resource;
    }
}