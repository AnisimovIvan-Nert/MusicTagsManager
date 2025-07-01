using Manager.Resource;

namespace Manager.Implementation.Resource;

public interface IResourceFactory
{
    IResource Create(string identifier, string name, string location);
}