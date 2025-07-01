using Manager.Resource;

namespace Manager.Implementation.Resource;

public class ResourceFactory : IResourceFactory
{
    public IResource Create(string identifier, string name, string location)
    {
        return new Resource(identifier, name, location);
    }
}