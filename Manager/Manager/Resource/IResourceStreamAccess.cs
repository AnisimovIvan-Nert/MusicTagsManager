namespace Manager.Resource;

public interface IResourceStreamAccess
{
    Stream OpenRead();
    Stream OpenWrite();
}