namespace MusicTagsManager.Resource;

public interface IResourceStreamAccess
{
    Stream OpenRead();
    Stream OpenWrite();
}