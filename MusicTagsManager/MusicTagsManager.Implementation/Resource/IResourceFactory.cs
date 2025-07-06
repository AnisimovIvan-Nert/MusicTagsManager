using MusicTagsManager.Resource;

namespace MusicTagsManager.Implementation.Resource;

public interface IResourceFactory
{
    IResource Create(string identifier, string name, string location);
}