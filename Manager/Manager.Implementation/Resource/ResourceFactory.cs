using Manager.Resource;

namespace Manager.Implementation.Resource;

public class ResourceFactory
{
    public IResource Create(string identifier, string name)
    {
        return new Resource(identifier, name);
    }
}